# Evidências de validação

Em 23/11/2026, em Linux com .NET SDK 10.0.100, a compilação Release foi concluída com zero erros e zero avisos. Dados de teste fictícios foram gravados em diretórios temporários.

| Validação | Resultado |
| --- | --- |
| Verificações de C# | 11 cenários aprovados, 0 falhas |
| Fluxo HTTP | 20 verificações aprovadas, 0 falhas |
| Reserva concorrente da última vaga | 1 aprovada de 8 tentativas |
| Emissão antes da presença | Recusada |
| Arquivo JSON inválido | Operação recusada e conteúdo preservado, com registro da exceção |
| Mediana de inscrição | 0,15 ms |
| Mediana de primeira emissão | 0,27 ms |

As medianas foram obtidas no serviço local, com 25 medições por operação após aquecimento. A medição inclui a persistência JSON e não inclui latência de navegação. O fluxo HTTP cobriu inscrição, duplicidade, papéis, token antifalsificação, atendimento, presença, aviso, saída, novo acesso, validação pública do certificado e gestão da programação.

## Responsividade

O roteiro `tests/responsividade.cjs` verifica a largura efetiva de sete páginas públicas e de acesso inicial, títulos, rótulos de controles e o foco do link de salto. O teste usa Chromium com Playwright nas larguras de 320, 390, 768 e 1.440 pixels. A execução local em Chromium 140 aprovou as 28 combinações de página e largura. Todas tiveram status 200, título único, rótulos associados e ausência de transbordamento horizontal. Em 320 px, Tab selecionou o link de salto e Enter transferiu o foco ao conteúdo.

| Largura | Evidência da página inicial |
| --- | --- |
| 390 px | [Celular](inicio-390.webp) |
| 768 px | [Tablet](inicio-768.webp) |
| 1.440 px | [Desktop](inicio-1440.webp) |

Para executar localmente com o portal ativo, instale as dependências de teste e execute os comandos na raiz do projeto.

```bash
npm install --no-save --package-lock=false playwright@1.58.2
npx playwright install chromium
mkdir -p capturas
CONECTA_CAPTURAS=capturas node tests/responsividade.cjs
```

A validação técnica é limitada aos cenários documentados. Ensaio de interpretação, compreensão em Libras e uso com leitores de tela fazem parte da validação com participantes.
