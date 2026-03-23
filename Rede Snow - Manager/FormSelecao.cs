using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace RedeSnow___Manager
{
    public partial class FormSelecao : Form
    {
        // Estrutura interna para ligar Código e Descrição
        public class ItemDefeito
        {
            public string Codigo { get; set; }
            public string Descricao { get; set; }
            public override string ToString() => Descricao; // O que aparece no Checkbox
        }

        private List<ItemDefeito> listaMestra = new List<ItemDefeito> {
            new ItemDefeito { Codigo = "C6", Descricao = "EQUIPAMENTO COM CARCAÇA DANIFICADA" },
            new ItemDefeito { Codigo = "D1", Descricao = "EQUIPAMENTO ACENDE SOMENTE LED POWER" },
            new ItemDefeito { Codigo = "C1", Descricao = "EQUIPAMENTO APRESENTANDO LOS" },
            new ItemDefeito { Codigo = "C2", Descricao = "EQUIPAMENTO APRESENTANDO SINAL RX ALTO" },
            new ItemDefeito { Codigo = "C3", Descricao = "EQUIPAMENTO APRESENTANDO SINAL RX BAIXO" },
            new ItemDefeito { Codigo = "C4", Descricao = "EQUIPAMENTO APRESENTANDO SINAL TX ALTO" },
            new ItemDefeito { Codigo = "C5", Descricao = "EQUIPAMENTO APRESENTANDO SINAL TX BAIXO" },
            new ItemDefeito { Codigo = "C7", Descricao = "EQUIPAMENTO COM CONECTOR PON DANIFICADO" },
            new ItemDefeito { Codigo = "D2", Descricao = "EQUIPAMENTO COM FALHA DE SOFTWARE" },
            new ItemDefeito { Codigo = "D3", Descricao = "EQUIPAMENTO COM FALHA NO WIFI 2.4G" },
            new ItemDefeito { Codigo = "D5", Descricao = "EQUIPAMENTO COM FALHA NO WIFI 2.4G/5G" },
            new ItemDefeito { Codigo = "D4", Descricao = "EQUIPAMENTO COM FALHA NO WIFI 5G" },
            new ItemDefeito { Codigo = "D6", Descricao = "EQUIPAMENTO COM PORTA DE TELEFONIA QUEIMADA" },
            new ItemDefeito { Codigo = "D7", Descricao = "EQUIPAMENTO COM PORTA LAN QUEIMADA" },
            new ItemDefeito { Codigo = "C8", Descricao = "EQUIPAMENTO NÃO PROVISIONA" },
            new ItemDefeito { Codigo = "D8", Descricao = "EQUIPAMENTO QUEIMADO" },
            new ItemDefeito { Codigo = "D9", Descricao = "EQUIPAMENTO REINICIANDO" },
            new ItemDefeito { Codigo = "D10", Descricao = "EQUIPAMENTO TRAVADO" },
            new ItemDefeito { Codigo = "C9", Descricao = "LEDS DE IDENTIFICAÇÃO DO EQUIPAMENTO QUEIMADOS" },
            new ItemDefeito { Codigo = "N3", Descricao = "EQUIPAMENTO ATUALIZADO PARA REDE NEUTRA, EQUIPAMENTO TESTADO E FUNCIONANDO" },
            new ItemDefeito { Codigo = "N1", Descricao = "EQUIPAMENTO COM A FIRMWARE ATUALIZADA, EQUIPAMENTO TESTADO E FUNCIONANDO" },
            new ItemDefeito { Codigo = "N2", Descricao = "EQUIPAMENTO TESTADO E FUNCIONANDO" }
        };

        // Agora retornamos objetos completos (Código + Descrição)
        public List<ItemDefeito> Selecionados { get; private set; } = new List<ItemDefeito>();
        private CheckedListBox clbDefeitos;
        private Button btnOk;

        public FormSelecao()
        {
            this.Text = "SELECIONE OS DEFEITOS";
            this.Size = new System.Drawing.Size(600, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            clbDefeitos = new CheckedListBox { Dock = DockStyle.Top, Height = 500, CheckOnClick = true };
            clbDefeitos.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            // Adiciona os itens na lista
            foreach (var item in listaMestra) clbDefeitos.Items.Add(item);

            btnOk = new Button { Text = "CONFIRMAR", Dock = DockStyle.Bottom, Height = 50, BackColor = System.Drawing.Color.DarkGreen, ForeColor = System.Drawing.Color.White };
            btnOk.Click += (s, e) => {
                Selecionados = clbDefeitos.CheckedItems.Cast<ItemDefeito>().ToList();
                this.DialogResult = DialogResult.OK;
            };

            this.Controls.Add(btnOk);
            this.Controls.Add(clbDefeitos);
        }
    }
}