using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Microsoft.VisualBasic;

namespace RedeSnow___Manager
{
    public partial class Form1 : Form
    {
        // --- CONFIGURAÇÕES DE VERSÃO E URLs ---
        private const string VersaoCompilada = "1.0.2";
        private const string UrlUpdate = "https://api.npoint.io/1deba0c7983047e931a3";
        private const string UrlBlockedSystem = "https://api.npoint.io/3205904552578c16bf73";

        private const string NomeArquivoLogs = "triagem_log.json";
        private const string ArquivoIPs = "ips_config.json";
        private const string ArquivoVersaoLocal = "versao_local.json";

        private List<RegistroEquipamento> logsGerais = new List<RegistroEquipamento>();
        private System.Windows.Forms.Timer timerRelogio;
        private System.Windows.Forms.Timer timerPing;

        public Form1()
        {
            InitializeComponent();

            // Sincroniza o arquivo local com a versão do código
            SincronizarVersaoLocal();

            this.Text = $"RedeSnow Manager - v{VersaoCompilada}";

            // Inicializa componentes de interface
            ConfigurarRelogio();
            ConfigurarTimerPing();
            ConfigurarLayoutPrompt();

            // Carrega dados locais
            CarregarLogsIniciais();
            CarregarIPsSalvos();
            AtualizarInterface();

            // Eventos ao carregar o formulário
            this.Load += async (s, e) =>
            {
                await ExecutarVerificacoesRemotas();
            };
        }

        // --- SISTEMA DE SEGURANÇA E ATUALIZAÇÃO ---
        private async System.Threading.Tasks.Task ExecutarVerificacoesRemotas()
        {
            // 1. Verifica Bloqueio primeiro
            await ValidarStatusSistema();

            // 2. Verifica Atualização depois
            VerificarAtualizacao();
        }

        private async System.Threading.Tasks.Task ValidarStatusSistema()
        {
            if (string.IsNullOrEmpty(UrlBlockedSystem)) return;

            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    string jsonString = await client.DownloadStringTaskAsync(UrlBlockedSystem);
                    var infoStatus = JsonSerializer.Deserialize<StatusModel>(jsonString);

                    if (infoStatus != null && infoStatus.status.ToLower() == "blocked")
                    {
                        if (timerPing != null) timerPing.Stop();

                        string msgUsuario = string.IsNullOrEmpty(infoStatus.mensagem)
                                            ? "O acesso a este sistema foi desativado pelo desenvolvedor."
                                            : infoStatus.mensagem;

                        MessageBox.Show(msgUsuario, "Sistema Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                        Application.Exit();
                        Environment.Exit(0); // Garante o fechamento imediato
                    }
                }
            }
            catch { /* Em caso de erro de rede, o sistema prossegue ou você pode decidir bloquear */ }
        }

        private async void VerificarAtualizacao()
        {
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    string json = await client.DownloadStringTaskAsync(UrlUpdate);
                    var dados = JsonSerializer.Deserialize<InfoUpdate>(json);

                    if (new Version(dados.versao) > new Version(VersaoCompilada))
                    {
                        string msg = $"Nova versão disponível: {dados.versao}\n\nNOTAS:\n{dados.notas}\n\nDeseja atualizar agora?";
                        if (MessageBox.Show(msg, "Atualizador", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            if (File.Exists("snowatualizador.exe"))
                            {
                                Process.Start("snowatualizador.exe", dados.url_zip);
                                Application.Exit();
                            }
                            else
                            {
                                MessageBox.Show("snowatualizador.exe não encontrado!");
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // --- SISTEMA DE PING (ESTILO TERMINAL) ---
        private void ConfigurarLayoutPrompt()
        {
            lstPromptPing.Font = new Font("Consolas", 9);
        }

        private void ConfigurarTimerPing()
        {
            timerPing = new System.Windows.Forms.Timer { Interval = 3000 };
            timerPing.Tick += async (s, e) =>
            {
                string ip = cbIPs.Text.Trim();
                if (string.IsNullOrEmpty(ip)) return;

                using (Ping p = new Ping())
                {
                    try
                    {
                        byte[] buffer = new byte[32];
                        var rep = await p.SendPingAsync(ip, 1000, buffer);
                        string log;

                        if (rep.Status == IPStatus.Success)
                        {
                            log = $"[{DateTime.Now:HH:mm:ss}] Resposta de {ip}: bytes=32 tempo={rep.RoundtripTime}ms TTL={rep.Options?.Ttl}";
                            lblStatusLan.Text = "EQUIPAMENTO CONECTADO";
                            lblStatusLan.ForeColor = Color.Green;
                        }
                        else
                        {
                            log = $"[{DateTime.Now:HH:mm:ss}] PING {ip} -> FALHA (Tempo Esgotado)";
                            lblStatusLan.Text = "EQUIPAMENTO DESCONECTADO";
                            lblStatusLan.ForeColor = Color.Red;
                        }

                        lstPromptPing.Items.Insert(0, log);
                        if (lstPromptPing.Items.Count > 50) lstPromptPing.Items.RemoveAt(50);
                    }
                    catch { }
                }
            };
            timerPing.Start();
        }

        // --- GESTÃO DE IPs ---
        private void btnAdicionarIP_Click(object sender, EventArgs e)
        {
            string novoIp = Interaction.InputBox("Digite o IP do equipamento:", "Cadastrar IP", "192.168.1.1");
            if (!string.IsNullOrWhiteSpace(novoIp) && !cbIPs.Items.Contains(novoIp))
            {
                cbIPs.Items.Add(novoIp);
                cbIPs.SelectedItem = novoIp;
                SalvarListaIPs();
            }
        }

        private void SalvarListaIPs() => File.WriteAllText(ArquivoIPs, JsonSerializer.Serialize(cbIPs.Items.Cast<string>().ToList()));

        private void CarregarIPsSalvos()
        {
            if (File.Exists(ArquivoIPs))
            {
                try
                {
                    var ips = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(ArquivoIPs));
                    if (ips != null) cbIPs.Items.AddRange(ips.ToArray());
                }
                catch { }
            }
        }

        // --- SALVAMENTO E TRIAGEM ---
        private void btnSalvar_Click(object sender, EventArgs e)
        {
            string id = txtIdentificador.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(id)) { MessageBox.Show("INSIRA O SERIAL/CODIGO/MAC!"); return; }

            using (var telaSelecao = new FormSelecao())
            {
                if (telaSelecao.ShowDialog() == DialogResult.OK && telaSelecao.Selecionados.Count > 0)
                {
                    string categoria = telaSelecao.Selecionados.Any(d => d.Codigo.StartsWith("N")) ? "NORMAL" : (telaSelecao.Selecionados.Any(d => d.Codigo.StartsWith("C")) ? "CARCAÇA" : "DEFEITO");

                    logsGerais.Add(new RegistroEquipamento
                    {
                        Identificador = id,
                        DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                        Categoria = categoria,
                        DefeitosBrutos = telaSelecao.Selecionados.Select(x => $"{x.Codigo}|{x.Descricao}").ToList()
                    });

                    File.WriteAllText(NomeArquivoLogs, JsonSerializer.Serialize(logsGerais, new JsonSerializerOptions { WriteIndented = true }));
                    AtualizarInterface();
                    txtIdentificador.Clear();
                    txtIdentificador.Focus();
                }
            }
        }

        private void btnGerar_Click_1(object sender, EventArgs e)
        {
            if (logsGerais.Count == 0) return;
            string caminhoTxt = Path.Combine(Application.StartupPath, "Relatorio_Triagem.txt");
            try
            {
                using (StreamWriter sw = new StreamWriter(caminhoTxt))
                {
                    sw.WriteLine($"RELATÓRIO DE TRIAGEM - {DateTime.Now:dd/MM/yyyy HH:mm}");
                    sw.WriteLine("==================================================");
                    foreach (var defeitoUnico in logsGerais.SelectMany(l => l.DefeitosBrutos).Distinct().OrderBy(d => d))
                    {
                        var partes = defeitoUnico.Split('|');
                        sw.WriteLine($"\n{partes[0]} \"{partes[1]}\"");
                        foreach (var s in logsGerais.Where(l => l.DefeitosBrutos.Contains(defeitoUnico)).Select(l => l.Identificador)) sw.WriteLine(s);
                    }
                }
                Process.Start(new ProcessStartInfo(caminhoTxt) { UseShellExecute = true });
            }
            catch (Exception ex) { MessageBox.Show($"ERRO: {ex.Message}"); }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("LIMPAR TUDO?", "CONFIRMAR", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                logsGerais.Clear();
                if (File.Exists(NomeArquivoLogs)) File.Delete(NomeArquivoLogs);
                AtualizarInterface();
            }
        }

        // --- AUXILIARES ---
        private void SincronizarVersaoLocal()
        {
            try
            {
                if (!File.Exists(ArquivoVersaoLocal) || JsonSerializer.Deserialize<VersaoModel>(File.ReadAllText(ArquivoVersaoLocal))?.versao != VersaoCompilada)
                    File.WriteAllText(ArquivoVersaoLocal, JsonSerializer.Serialize(new VersaoModel { versao = VersaoCompilada }));
            }
            catch { }
        }

        private void ConfigurarRelogio()
        {
            timerRelogio = new System.Windows.Forms.Timer { Interval = 1000 };
            timerRelogio.Tick += (s, e) => { lblHorario.Text = DateTime.Now.ToString("HH:mm:ss"); };
            timerRelogio.Start();
        }

        private void AtualizarInterface()
        {
            lblTotal.Text = $"TOTAL HOJE: {logsGerais.Count}";
            dgvLogs.DataSource = null;
            dgvLogs.DataSource = logsGerais.Select(l => new {
                DATA = l.DataHora,
                SERIAL = l.Identificador,
                CATEGORIA = l.Categoria,
                DIAGNOSTICO = string.Join(", ", l.DefeitosBrutos.Select(d => d.Split('|')[1]))
            }).ToList();
            if (dgvLogs.Columns.Count > 0) { dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; dgvLogs.Columns["DIAGNOSTICO"].FillWeight = 200; }
        }

        private void CarregarLogsIniciais() { if (File.Exists(NomeArquivoLogs)) try { logsGerais = JsonSerializer.Deserialize<List<RegistroEquipamento>>(File.ReadAllText(NomeArquivoLogs)) ?? new List<RegistroEquipamento>(); } catch { } }
    }

    // --- CLASSES DE MODELO (MODELS) ---
    public class StatusModel { public string status { get; set; } = "free"; public string mensagem { get; set; } = ""; }
    public class VersaoModel { public string versao { get; set; } = ""; }
    public class InfoUpdate { public string versao { get; set; } = ""; public string url_zip { get; set; } = ""; public string notas { get; set; } = ""; }
    public class RegistroEquipamento { public string Identificador { get; set; } = ""; public string DataHora { get; set; } = ""; public string Categoria { get; set; } = ""; public List<string> DefeitosBrutos { get; set; } = new List<string>(); }
}