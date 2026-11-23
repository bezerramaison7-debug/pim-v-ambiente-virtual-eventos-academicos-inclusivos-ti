# PIM V — Ambiente virtual responsivo para eventos acadêmicos inclusivos em TI

Conecta TI é o protótipo do Projeto Integrado Multidisciplinar V de Análise e Desenvolvimento de Sistemas. O evento fictício tem quatro atividades, inscrições, atendimento, gestão de presença, certificados verificáveis e storyboards de orientações em Libras.

## Executar no computador

Instale o [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0). A aplicação usa ASP.NET Core MVC e arquivos JSON. Execute na raiz do projeto.

**Windows (PowerShell)**

```powershell
$env:CONECTA_ADMIN_SENHA = 'PimV-Demonstracao-2026'
dotnet run --project src/ConectaTI --urls http://localhost:5178
```

**Linux e macOS**

```bash
export CONECTA_ADMIN_SENHA='PimV-Demonstracao-2026'
dotnet run --project src/ConectaTI --urls http://localhost:5178
```

Abra [localhost:5178](http://localhost:5178). A senha é configurada aqui apenas para a demonstração local. A gestão aceita uma senha definida no ambiente com pelo menos 12 caracteres. Use e-mails fictícios como `demo@example.test`.

## Percurso de avaliação

1. Explore a página inicial, filtre a programação e abra os storyboards em **Libras**.
2. Faça uma inscrição, guarde a chave de 32 caracteres exibida após a confirmação e envie uma mensagem pela área do participante.
3. Em outra janela privada, acesse [Comissão organizadora](http://localhost:5178/gestao) com a senha configurada. Localize a inscrição, confirme a presença e responda à mensagem.
4. Volte à área do participante, atualize a página e clique em **Emitir certificado**. Abra o certificado, salve em PDF pelo comando de impressão e copie o código.
5. Na janela sem sessão do participante, consulte o código em **Certificados**. No painel, experimente publicar um aviso e editar ou cancelar uma atividade.

O cadastro é persistido na primeira gravação em `src/ConectaTI/App_Data/evento.json`. Use `CONECTA_DADOS` para definir outro caminho e isolar demonstrações. Para começar limpo, encerre o servidor e use um caminho novo.

## Validação

```bash
dotnet build src/ConectaTI -c Release
dotnet run --project tests/ConectaTI.Checks -c Release
```

Com o servidor apontado para um arquivo de teste novo, a senha exportada e o Python 3 disponível, execute `python tests/fluxo_http.py`. O fluxo cria registros fictícios. A URL padrão é `http://127.0.0.1:5178` e pode ser ajustada com `CONECTA_TESTE_URL`. O workflow de GitHub Actions também executa essas verificações.

## Entregáveis

- [Relatório acadêmico em PDF](docs/PIM_V_Conecta_TI.pdf) e [versão editável](docs/PIM_V_Conecta_TI.docx).
- [Especificação, fluxos e guia técnico](docs/tecnico.md).
- [Plano de comunicação, liderança e negociação](docs/comunicacao.md).
- [Relatório e storyboards de Libras](docs/libras.md).
- [Resultados de validação e responsividade](docs/evidencias/validacao.md).

O evento, os nomes e os materiais de avaliação são demonstrativos. A agenda pode apontar para salas HTTPS externas cadastradas pela comissão. O sistema usa um único processo de aplicação, arquivo local e sessão por cookie. Atendimento, avisos e confirmação de inscrição funcionam dentro do portal. O capítulo 6 do relatório descreve os limites do protótipo.
