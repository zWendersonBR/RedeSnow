
# ❄️ RedeSnow Manager

![Versão](https://img.shields.io/badge/version-1.0.1-blue)
![C#](https://img.shields.io/badge/C%23-DotNet-purple)
![License](https://img.shields.io/badge/license-MIT-green)

**RedeSnow Manager** é uma ferramenta robusta para gestão de triagem de equipamentos, monitoramento de rede em tempo real e controle remoto de acesso. Desenvolvido em C# WinForms, o software foca em produtividade para técnicos de bancada e administradores de rede.

---

## 🚀 Funcionalidades Principais

### 📋 Triagem e Log de Defeitos
* **Registro por Serial:** Identificação única para cada equipamento.
* **Categorização Automática:** Separa equipamentos entre `NORMAL`, `CARCAÇA` ou `DEFEITO` com base nos códigos selecionados.
* **Exportação de Relatórios:** Gera arquivos `.txt` organizados por tipo de defeito para fácil conferência.
* **Persistência de Dados:** Armazenamento automático em JSON local (`triagem_log.json`).

### 🌐 Monitoramento de Rede (Ping Real-time)
* **Interface de Terminal:** Console integrado no estilo CMD (preto e verde).
* **Dados Detalhados:** Exibe o tempo de resposta em milissegundos (ms) e o TTL (Time to Live) de cada pacote.
* **Gestão de IPs:** Permite cadastrar e salvar uma lista de IPs frequentes para monitoramento cíclico.

### 🛡️ Segurança e Manutenção
* **Bloqueio Remoto:** O administrador pode desativar o uso do software instantaneamente através de um JSON remoto.
* **Sistema de Auto-Update:** Verifica novas versões no início da execução e baixa pacotes de atualização automaticamente via `snowatualizador.exe`.
* **Sincronização de Versão:** Garante que o ambiente local esteja sempre alinhado com a versão compilada.

---

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C# (.NET Framework)
* **Interface:** Windows Forms (WinForms)
* **Serialização:** `System.Text.Json`
* **Rede:** `System.Net.NetworkInformation`
* **Serviços Externos:** JSON Hosting (npoint.io ou similar)

---

## 📦 Como Instalar

1. Faça o download da última [Release](link-para-sua-release).
2. Certifique-se de que o arquivo `snowatualizador.exe` está na mesma pasta que o `RedeSnowManager.exe`.
3. Execute o programa. Ele criará automaticamente os arquivos de configuração:
   - `ips_config.json`
   - `versao_local.json`

---

## ⚙️ Configuração do Administrador

Para gerenciar o bloqueio e as atualizações, você deve manter dois arquivos JSON online:

### 1. JSON de Atualização (`UrlUpdate`)
```json
{
  "versao": "1.0.1",
  "url_zip": "[https://seu-link.com/update.zip](https://seu-link.com/update.zip)",
  "notas": "Correção no ping e novos códigos de triagem."
}
```

### 2. JSON de Status (`UrlBlockedSystem`)
```json
{
  "status": "free",
  "mensagem": "Sistema liberado para uso."
}
```
*Mude para `"status": "blocked"` para impedir o acesso de todos os usuários.*

---

## 🤝 Contribuição

1. Faça um Fork do projeto.
2. Crie uma Branch para sua Feature (`git checkout -b feature/NovaFeature`).
3. Commit suas mudanças (`git commit -m 'Adicionando nova funcionalidade'`).
4. Push para a Branch (`git push origin feature/NovaFeature`).
5. Abra um Pull Request.

---

## ⚖️ Licença

Distribuído sob a licença MIT. Veja `LICENSE` para mais informações.

---
