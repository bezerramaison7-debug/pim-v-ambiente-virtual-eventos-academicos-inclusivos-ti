# 1 INTRODUÇÃO

Eventos acadêmicos em Tecnologia da Informação permitem que estudantes compartilhem projetos, pratiquem conteúdos das disciplinas e entrem em contato com profissionais. Quando o evento acontece online, a divulgação, a programação e o atendimento passam a depender da infraestrutura digital. Para uma pessoa que utiliza Libras, informações disponíveis somente em áudio podem prejudicar tanto o planejamento da participação quanto o acompanhamento de mudanças durante a edição.

O objetivo geral deste trabalho é desenvolver e avaliar um protótipo responsivo para gestão de um evento acadêmico inclusivo em TI. O projeto foi materializado no Conecta TI, encontro online fictício com atividades de desenvolvimento de software e acessibilidade. A proposta integra uma interface web em HTML e CSS, regras de negócio orientadas a objetos em C#, um plano de comunicação da comissão e materiais de pré-produção em Libras.

Os objetivos específicos são viabilizar consulta à programação, inscrição, atendimento, confirmação de presença e emissão de certificado; descrever os papéis da comissão e seus protocolos de comunicação; estruturar um glossário contextual e três storyboards; e documentar requisitos, decisões técnicas, cenários de teste e resultados.

O trabalho segue uma abordagem aplicada. O manual do PIM V orientou a delimitação dos entregáveis. A documentação de desenvolvimento web e as referências sobre acessibilidade, negociação e Libras fundamentaram as decisões. O protótipo foi avaliado por verificações de regras de negócio e por um fluxo HTTP que exercita os formulários. A avaliação de responsividade contempla larguras de celular, tablet e desktop. O capítulo de resultados apresenta as evidências disponíveis e explicita os limites para generalização.

O desenvolvimento descreve o cenário adotado, os requisitos e os fluxos. Em seguida, examina a interface, a modelagem em C#, a comunicação, a negociação e os recursos em Libras. A conclusão retoma os resultados e as condições necessárias para uma aplicação institucional.

# 2 CONTEXTO, REQUISITOS E FLUXOS

## 2.1 Caracterização do Conecta TI

O Conecta TI é uma edição acadêmica online, gratuita e fictícia, programada para os dias 5 e 6 de dezembro de 2026, no horário de Brasília. A capacidade inicial de 120 inscrições é um parâmetro de demonstração. A programação contém uma abertura, um minicurso, uma oficina e uma mesa-redonda. Os dois primeiros blocos apresentam acessibilidade e interfaces. Os dois últimos abordam programação em C# e práticas de inclusão em eventos.

A comissão precisa divulgar horários, acolher dúvidas, confirmar o comparecimento e emitir certificados. Visitantes precisam descobrir o evento, identificar seu formato, conhecer os apoios previstos e decidir se podem participar. Pessoas já inscritas precisam retomar o acesso, acompanhar respostas e conferir a situação de sua presença. A proposta adota vocabulário comum nas páginas para que programadores, mediadores, participantes e avaliadores compreendam a mesma sequência.

A agenda explicita o horário, a duração, a pessoa convidada e a previsão de recursos de acesso em cada atividade. Um link HTTPS pode apontar para a sala externa cadastrada pela comissão. Uma edição fictícia mantém a demonstração independente de agenda, equipe ou serviços externos reais. O material de apresentação indica essa condição e utiliza dados de avaliação.

O protótipo reúne inscrição e comunicação no portal. A ferramenta privilegia a continuidade do percurso entre o convite e a entrega do certificado. Em uma situação de alteração de horário, por exemplo, a comissão pode atualizar a agenda, registrar aviso público e responder em canal privado a quem tiver solicitado apoio. O controle de presença é administrado pela comissão. Essa decisão define o ponto em que o trabalho humano e a regra automática se encontram.

São considerados como resultados a operação funcional dos fluxos, a consistência dos registros e a documentação integrada das quatro disciplinas. A organização do evento serve como situação de estudo para relacionar acessibilidade, regras de negócio e comunicação com os participantes.

# 2.2 Requisitos funcionais e critérios de aceite

A página inicial apresenta a edição e a próxima ação disponível. A programação ordena atividades por horário, separa as datas e permite selecionar um formato. A comissão consegue cadastrar atividades, editar os horários e marcar cancelamento de forma explícita. Caso tenha endereço de sala, informa um URL HTTPS. A atividade cancelada continua visível com indicação textual e perde o acesso à sala.

A inscrição coleta nome para o certificado e e-mail. O servidor valida a entrada, normaliza o e-mail e verifica duplicidade e capacidade. A pessoa que conclui a inscrição recebe confirmação e chave de acesso. O aceite desse requisito inclui rejeitar uma segunda inscrição para o mesmo e-mail, inclusive quando se altera a caixa das letras.

O atendimento permite enviar dúvida, feedback ou pedido de apoio. A resposta da comissão fica disponível na área do participante. Avisos de interesse coletivo aparecem em páginas públicas. A separação entre aviso e mensagem dá à equipe um local apropriado para cada tipo de conteúdo. A informação individual deve permanecer acessível ao titular do registro e à organização responsável.

A emissão depende de presença confirmada. Um certificado inclui nome, título do evento, período, duração, data de emissão e código. Quando a emissão é repetida, a aplicação recupera o mesmo documento. O código permite consulta pública e a folha de estilo de impressão permite salvar em PDF pelo navegador.

Requisitos de qualidade incluem navegação por teclado, rótulos associados a campos, ausência de transbordamento horizontal no tamanho mínimo verificado, persistência em arquivo e registro de operações. A presença de storyboards, glossário e encaminhamento ao atendimento integra o requisito de apoio em Libras. O documento técnico do repositório relaciona cada requisito a uma interface e a um resultado verificável. A comprovação de acessibilidade linguística exige avaliação própria com pessoas surdas e profissionais de Libras.

# 2.3 Atores, casos de uso e estados

O visitante consulta o início, a programação, o glossário e a verificação de certificados. A pessoa inscrita tem acesso à área privada. A comissão possui credencial administrativa e executa atividades de coordenação, atendimento e registro de presença. A verificação pública por código pode ser feita por outra pessoa que receba o certificado.

No primeiro fluxo, o visitante abre o formulário, envia nome e e-mail e acompanha a confirmação. Entradas inválidas, duplicidade, encerramento ou falta de vaga retornam mensagens com a causa. O cadastro bem-sucedido inicia uma sessão autenticada. Uma sessão posterior pode ser iniciada pela combinação de e-mail e chave. A comissão não vê a chave pessoal no painel.

No segundo fluxo, a pessoa autenticada envia uma solicitação. A comissão recebe o texto e publica uma resposta privada. A pessoa retorna à própria área e lê o resultado. Uma solicitação de legenda, por exemplo, pode motivar um aviso público sobre material de apoio para toda a turma. A publicação do aviso é uma decisão editorial própria, que deve omitir dados do atendimento individual.

No terceiro fluxo, a comissão localiza a inscrição pelo nome, verifica a participação pelo protocolo da edição e grava a presença. A área privada passa da situação de espera para a possibilidade de emitir. A emissão confirma o estado no servidor, calcula a duração disponível e cria um código. O certificado pode ser exibido a partir da área privada ou consultado por código.

A revisão desses casos permitiu identificar pontos de falha e estados de interface. Uma visita direta à área privada apresenta a tela de acesso. Uma ação administrativa sem o papel correspondente é recusada. Um código inexistente exibe a situação de consulta. A documentação técnica registra as relações entre casos de uso, requisitos e exceções para permitir a reconstrução dos percursos em avaliação.

# 3 DESENVOLVIMENTO WEB RESPONSIVO

## 3.1 Estrutura das páginas e navegação

A interface é servida por ASP.NET Core MVC. Views Razor interpolam o estado obtido do servidor em HTML5. As páginas de início, agenda, inscrição, área do participante e certificados realizam o percurso principal. A página Libras dá acesso a orientações bilíngues em protótipo. A comissão utiliza uma área de acesso e um painel com seções de gestão. A documentação da Microsoft (s.d.) descreve a separação entre controllers, modelos e views no ASP.NET Core MVC, adotada para manter validações e apresentação em lugares distintos.

O cabeçalho oferece navegação persistente. O início apresenta título, período e chamadas para inscrição e programação. No rodapé, o acesso de gestão preserva o foco do cabeçalho nas tarefas do público. Páginas internas usam um título principal e seções com níveis subsequentes para permitir leitura sequencial e localização do conteúdo.

A agenda usa grupos por dia e cartões de atividade. Cada cartão conserva horário, duração, título, descrição e acessibilidade em uma ordem coerente. O filtro por formato usa controle nativo e funciona por requisição GET, o que permite uma URL para o resultado. O estado vazio orienta a pessoa a mudar a seleção. Avisos aparecem após a agenda e na página inicial.

Formulários usam elementos semânticos com rótulos e limites de entrada. O fluxo funciona com renderização no servidor. Após mutações em áreas autenticadas, um redirecionamento retorna à página atualizada e exibe uma mensagem de resultado. Na inscrição inicial, a página de confirmação apresenta a chave uma vez. O painel usa seções com âncoras para que a comissão chegue a tarefas frequentes sem percorrer todos os formulários.

O certificado mantém o título e as informações de verificação visíveis em tela. Em impressão, navegação, rodapé e formulário ficam ocultos pela folha de estilo, permitindo usar a função de impressão do navegador para salvar o documento.

# 3.2 Layout fluido e critérios de responsividade

A largura do conteúdo usa uma dimensão máxima com margens flexíveis. O objetivo é aproveitar monitores largos sem estender linhas de leitura por toda a janela. Componentes baseados em CSS Grid organizam os destaques, fatos do evento, agenda e glossário. Flexbox permite que navegação, filtros e ações quebrem a linha quando o conteúdo cresce.

Em telas largas, o início coloca apresentação textual e grafismo em colunas. As atividades em destaque ocupam três colunas. Em larguras intermediárias, os cartões passam para duas colunas e o espaço da ilustração recebe proporção menor. Na largura de celular, o conteúdo passa para uma coluna, o cabeçalho deixa os links de navegação em uma linha própria e o horário aparece acima das informações de cada atividade. Os pontos de mudança são 53 e 40 rem. A unidade relativa permite adaptar o comportamento quando o tamanho padrão de fonte muda.

O texto longo usa regras de quebra, importantes em nomes, mensagens e chaves. Campos possuem largura máxima de 100% e os grids usam colunas com mínimo zero para reduzir transbordamento. Botões contêm ações curtas e reorganizam sua posição com a largura disponível. O tema visual usa fundo claro na maior parte da experiência, painel escuro no início e cor de destaque nos controles. O grafismo inicial é decorativo e está fora da árvore de acessibilidade.

O critério 1.4.10 das WCAG 2.2 orienta a apresentação de conteúdo com reflow em largura equivalente a 320 pixels CSS (W3C, 2024). A verificação automatizada compara a largura do documento com a área de visualização em 320, 390, 768 e 1.440 pixels. As larguras representam extremos e transições úteis para avaliação. Capturas do início em celular, tablet e desktop complementam a medição. Verificações automatizadas descrevem o estado do protótipo nesses cenários e não equivalem a uma avaliação visual de todo conteúdo futuro.

# 3.3 Teclado, semântica e feedback

As WCAG 2.2 organizam critérios de acessibilidade para conteúdos web, incluindo operação por teclado, contraste, mudanças de foco e identificação de erros (W3C, 2024). No Conecta TI, links, botões, campos, seleção de formato e painéis expansíveis utilizam controles nativos. Isso mantém ativação por teclado e semântica compatível com tecnologias assistivas. A visualização independente de um menu com script reduz dependências no percurso principal.

O primeiro elemento focável é o link Pular para o conteúdo, exibido quando recebe foco. A ativação move o foco para a região principal. Estados ativos da navegação pública incluem indicação de página corrente. O foco tem contorno visível e os textos de ação apresentam nomes próprios. Na gestão, o botão de presença inclui o nome da pessoa no nome acessível, para distinguir ações idênticas na lista.

Mensagens de erro ou confirmação aparecem em uma região identificável. Abaixo do título da agenda, o horário de Brasília é apresentado em texto. Um cancelamento inclui a palavra Cancelada. Recursos previstos são descritos por extenso em cada cartão. Essa redundância permite entender os estados independentemente da percepção de cor.

O projeto adota contraste de texto por seleção de paleta e mantém estados com contraste visual, inclusive no painel escuro de abertura. O ícone de marca e o grafismo do início são decorativos, com alternativa informativa fornecida pelo texto adjacente. Formulários usam rótulos explícitos e instruções sobre a finalidade dos dados, como o uso do nome no certificado.

A interação com leitor de tela e a navegação com ampliação em dispositivos próprios são observações complementares para avaliação futura. A validação automática verifica elementos observáveis e pode detectar transbordamento ou ausência de rótulos. Ela não avalia disponibilidade de interpretação em cada atividade nem mede compreensão de conteúdo em Libras. Essas dimensões exigem ensaio com a comissão, profissionais de Libras e pessoas participantes.

# 4 PROGRAMAÇÃO ORIENTADA A OBJETOS EM C#

## 4.1 Entidades e encapsulamento

Evento representa uma edição. Reúne título, período exibido, capacidade e estado das inscrições. O método de configuração recusa uma capacidade abaixo da ocupação atual. Atividade representa um item da agenda com formato, convidado, início, duração, descrição, recursos previstos, situação de cancelamento e sala opcional. A criação valida intervalos de duração e data e o esquema do endereço de sala.

Participante tem identificador, nome, e-mail e hash da chave. Inscrição liga a pessoa à edição por meio do estado agregado do projeto. Sua propriedade de presença tem escrita privada, com transição explícita pelo método ConfirmarPresenca. O certificado armazena um retrato do evento no momento da emissão. Isso permite preservar a carga horária e o título para documentos emitidos antes de futuras mudanças da agenda.

UsuarioAdministrador representa o papel da comissão e verifica a senha definida por configuração de ambiente. A autenticação do participante usa chave gerada aleatoriamente pelo aplicativo. A senha administrativa precisa ser configurada para o ciclo local de testes. A sessão distingue os papéis por declarações de autorização no controller e é encerrada pelo usuário ao selecionar Sair.

As entidades auxiliares tratam avisos, mensagens e auditoria. Uma mensagem guarda a referência da pessoa participante e a resposta da comissão. O aviso tem tipo, texto e instante de publicação. A auditoria guarda operação, referência quando aplicável e instante. A relação entre essas entidades acompanha os casos de uso e mantém a lógica interna expressa no vocabulário do evento.

A modelagem usa composição e a interface IRepositorio para expressar o contrato de persistência. O serviço consome essa interface e o repositório JSON implementa as operações. Os controllers usam os métodos do serviço, o que permite testar regras independentemente da interface. A escolha atende à exposição de interfaces e ao encapsulamento indicados pelo manual.

# 4.2 Serviços, coleções e LINQ

EventosService realiza as operações de negócio utilizadas pelos controllers. Ele decide se a inscrição pode ser aberta para um e-mail, se a pessoa autenticada pode receber um certificado e quais registros entram no resultado de uma pesquisa. Validações de formato aparecem no serviço ou na entidade. Controllers se concentram no estado da requisição, na autenticação, no modelo de tela e na mensagem de retorno.

O estado persistido contém listas genéricas de atividades, participantes, inscrições, certificados e registros de comunicação. O protótipo materializa essas coleções em memória a cada operação. Na agenda, Where seleciona atividades do formato escolhido e OrderBy ordena pelo início. Na gestão, Join combina a inscrição ao participante por identificador. Where aplica uma busca que ignora caixa e OrderBy organiza os resultados pelo nome em ordem adequada ao português. A área privada filtra mensagens pelo identificador da sessão.

A identificação por GUID liga entidades e reduz o acoplamento ao nome ou ao e-mail. O e-mail é normalizado e usado para impedir cadastro duplicado. A chave de acesso é diferente do código do certificado. A primeira autentica a pessoa com e-mail e a segunda permite consulta do documento público quando compartilhada. O servidor usa um cookie de autenticação para encaminhar as requisições autenticadas.

Para reduzir efeitos de tentativas repetidas, a confirmação de presença só registra uma nova entrada de auditoria quando ocorre a transição. A emissão reutiliza o certificado encontrado e verifica de novo a existência dentro da operação de gravação quando necessário. A duração é calculada pela soma das atividades ativas no instante da emissão e deve ser positiva. Uma alteração futura de duração só poderá afetar novas emissões.

O uso de um provedor de tempo permite concentrar a captura de instantes e manter data de operação em UTC. As telas mostram instantes do evento em horário de Brasília. A duração e os horários apresentados fazem parte das regras simuladas da edição.

# 4.3 Persistência, integridade e tratamento de exceções

O repositório guarda o estado em JSON em um arquivo local. O código de inicialização fornece atividades e um aviso de demonstração quando o arquivo está ausente. A primeira mutação grava o estado completo. Um caminho alternativo pode ser configurado para isolar teste e uso de demonstração. O diretório de dados fica fora da pasta de arquivos servidos publicamente e fora dos arquivos versionados.

Cada operação de alteração ocorre sob trava no mesmo processo. O repositório lê o estado atual, aplica a regra, grava em arquivo temporário no mesmo diretório, descarrega o buffer e move a versão concluída para o destino. Se a validação de negócio falha, a operação termina antes de escrever o arquivo. Em uma disputa pela última vaga, requisições concorrentes processadas na mesma instância observam o estado atualizado antes da próxima reserva.

Falhas de leitura, gravação e interpretação de JSON são encaminhadas ao log do servidor. O usuário recebe mensagem compreensível no fluxo correspondente ou uma página de erro. O teste de corrupção escreve deliberadamente JSON inválido e verifica que a operação retorna o erro de armazenamento sem substituir os bytes. As ações administrativas recebem mensagens para dados inválidos e registram auditoria após a alteração bem-sucedida.

A segurança da demonstração inclui token antifalsificação nos formulários, cookies, restrição por papel e limite de tentativas nos acessos. O participante usa chave aleatória de 128 bits, cujo hash é persistido, e pode sair da sessão. A tela de consulta por código apresenta o certificado, inclusive o nome, para quem conhece o código. O compartilhamento desse código deve ser controlado pela pessoa participante.

A estratégia de arquivo é adequada a uma demonstração executada em instância única e com baixo volume. Uma implantação real precisaria definir políticas para proteção e retenção de dados, recuperação de acesso, cópias de segurança, monitoração e tratamento de acessos simultâneos em múltiplos processos.

# 5 COMUNICAÇÃO, LIDERANÇA E NEGOCIAÇÃO

## 5.1 Públicos, objetivos e responsabilidades

A comissão organiza a comunicação em três momentos. Antes do encontro, divulga as atividades, confirma a reserva e alinha recursos com palestrantes e intérpretes. Durante o evento, acompanha mudanças, atende dúvidas e confirma ocorrências. Após a edição, libera certificados, responde pendências e analisa retorno para orientar o próximo planejamento.

Participantes da comunidade acadêmica e do público externo recebem divulgação com data, formato, link de inscrição e informações de acesso. Portal, redes sociais e e-mail institucional podem compartilhar o convite. O portal mantém os horários e avisos vigentes. A curadoria utiliza reuniões breves e confirmação escrita com palestrantes para definir título, duração, materiais e disponibilidade de recursos. Patrocinadores e apoiadores recebem informações sobre contrapartidas acordadas e autorização para divulgação.

A coordenação do curso acompanha programação, carga horária, regras de certificação e síntese pós-evento. A equipe técnica e a equipe de acessibilidade ensaiam navegação, salas externas, legendas, enquadramento da pessoa intérprete e recursos de contingência. Um checklist documenta responsáveis, horário e condições de funcionamento. A equipe de atendimento usa a área de gestão e mantém as respostas individuais no portal.

A liderança da comissão atribui decisões e limita concentração de tarefas. Curadoria valida programação, comunicação edita avisos, técnica confirma alterações de sala e acessibilidade verifica o recurso anunciado. Em incidentes, uma pessoa de referência é responsável por acompanhar a demanda até a resolução, pedir apoio e publicar o retorno aprovado.

Como metas do plano, o convite é preparado duas semanas antes, o ensaio ocorre dois dias antes e o resumo de fechamento pode ser preparado três dias depois. Durante atividades, solicitações de acessibilidade recebem acolhimento prioritário, com meta de primeiro retorno em dez minutos. O cronograma expressa o procedimento proposto para uma edição real. O protótipo demonstra os canais internos e fornece textos reutilizáveis pela comissão.

# 5.2 Mensagens, avisos e atendimento

O convite apresenta objetivo, período, formato e ação. Um exemplo para a edição é: participe do Conecta TI nos dias 5 e 6 de dezembro, online e com inscrição gratuita. Consulte os recursos previstos na programação e reserve sua vaga. O texto curto pode ser adaptado para redes e usado como referência do storyboard de convite.

Na confirmação, a pessoa recebe o status da reserva e a orientação para guardar a chave exibida. Mudanças posteriores devem indicar atividade, horário de Brasília e ação esperada. Para a oficina, um aviso poderia comunicar que a nova abertura é às 10h e que o material atualizado está na programação. No caso de atraso, o texto registra previsão e instante da próxima atualização. A informação sobre cancelamento indica o item atingido e a existência de reposição, quando definida.

Em um incidente, a equipe técnica confirma fatos. A curadoria verifica o efeito na agenda e a equipe de acessibilidade revisa o impacto nos recursos anunciados. Comunicação atualiza a programação e publica aviso em seguida. Quando já houve divulgação em redes ou e-mail, esses canais repetem a mensagem essencial e apontam para a agenda atualizada. O registro da decisão contém data, responsável, justificativa e próximo acompanhamento.

O atendimento privado acolhe solicitações e mantém o histórico entre mensagem e resposta. A comissão pode confirmar recebimento, indicar prazo e pedir apenas os dados necessários para tratar o pedido. O retorno final informa a solução disponível, o canal de acesso e a pessoa de referência, quando apropriado. A equipe protege a identificação individual ao aproveitar aprendizados do atendimento para avisos coletivos.

Na etapa de avaliação, o número de solicitações, o tipo de barreira relatada e a capacidade de resposta ajudam a escolher melhorias. Os relatórios podem usar resultados agregados. Comentários pessoais, informações de saúde e endereços de contato não integram os avisos públicos do encontro.

# 5.3 Negociação integrativa e gestão de conflitos

Fisher, Ury e Patton (2011) organizam a negociação por princípios em torno da compreensão de interesses, da construção de opções e do uso de critérios objetivos. No contexto do Conecta TI, essa abordagem orienta decisões que afetam agenda, recursos de acessibilidade e certificação. A liderança registra o encaminhamento e informa prazos e consequências a quem participa da decisão.

O primeiro cenário é a disputa de horário entre dois palestrantes. A curadoria escuta cada pessoa, identifica restrições de trabalho e disponibilidade, resume os interesses e apresenta os horários em que os recursos de acesso estão confirmados. Os critérios incluem previsibilidade para o público, continuidade da grade e disponibilidade da mediação e da interpretação. Entre as opções estão inverter blocos, trocar formato ou reagendar uma apresentação. A solução pactuada é registrada, aplicada na agenda e comunicada no portal.

O segundo cenário é uma solicitação de apoio adicional em Libras para uma atividade. A equipe de atendimento investiga em canal privado qual tarefa está sendo prejudicada e qual apoio a pessoa considera efetivo. Coordenação, técnica e acessibilidade analisam as alternativas, que podem envolver confirmação de cobertura, material prévio e forma de participação em texto. A resposta deve explicitar a cobertura disponível e combinar um encaminhamento com a pessoa interessada. Quando não houver recurso equivalente, a comissão registra a limitação e o impacto para a decisão de prosseguir ou reagendar.

Um terceiro cenário trata de divergência sobre presença. A coordenação compara o registro da comissão com evidências do mediador, ouve a pessoa e aplica os mesmos critérios divulgados para casos equivalentes. O resultado é comunicado em canal privado. Se houver erro administrativo, a comissão registra a correção.

No fechamento, uma reunião relaciona ocorrências, respostas, certificados e mudanças feitas na edição. Para o ciclo seguinte, cada melhoria recebe prioridade com base em frequência e impacto, pessoa responsável e prazo de nova verificação.

# 6 LIBRAS E ACESSIBILIDADE LINGUÍSTICA

## 6.1 Fundamentação e glossário contextual

A Lei nº 10.436/2002 reconhece Libras como meio legal de comunicação e expressão e descreve sua estrutura gramatical própria. O Decreto nº 5.626/2005 regulamenta a Lei e trata da presença da língua em contextos de educação e acesso (Brasil, 2002, 2005). A proposta bilíngue do Conecta TI preserva informações de navegação em português escrito e organiza conteúdos em Libras para a produção orientada por profissionais e pela comunidade surda.

O glossário delimita os sentidos de termos que podem ser confundidos. Inscrição significa reservar vaga. Credenciamento trata de identificação no acesso e presença corresponde ao registro administrativo de comparecimento. Certificado representa o documento gerado após a confirmação. Chave pessoal é uma credencial. Código de verificação é o identificador destinado à consulta da autenticidade do certificado. A distinção aparece no storyboard de inscrição e no storyboard de navegação.

Palestra, minicurso e mesa-redonda recebem exemplos situados na agenda. A palestra apresenta um tema por uma pessoa convidada. O minicurso envolve aprendizagem guiada e a mesa-redonda envolve contribuições mediadas de várias pessoas. Cancelamento deve ser tratado junto da diferença semântica entre atraso e mudança de horário. As três situações podem conduzir a ações diferentes na agenda.

Para termos de TI, como código-fonte e acessibilidade digital, o roteiro propõe primeiro apresentar o conceito em contexto. Um recorte de código ou a navegação por teclado ancoram o sentido. O Dicionário Libras do INES (s.d.) é uma referência para consulta visual. Variantes e termos especializados devem ser discutidos com pessoas surdas fluentes e profissionais que conheçam o domínio de TI.

A entrega é um protótipo de pré-produção, com sentidos, sequências visuais e indicação de trabalho linguístico. Os roteiros fornecem uma base comum para revisão e gravação. A validação deve considerar variação linguística, compreensão da ação e adequação do conteúdo ao público acadêmico.

# 6.2 Storyboards de convite, inscrição e orientação

O storyboard de convite abre com a identificação do Conecta TI e uma cartela lateral. Em seguida, apresenta data, modalidade online e exemplos de atividades. O terceiro quadro mostra a página inicial e indica o botão Fazer inscrição. A duração estimada é de 35 segundos. O propósito é permitir que a pessoa distinga proposta, quando acontece e como iniciar o processo.

No storyboard de inscrição, os primeiros 12 segundos mostram o caminho até o formulário. De 12 a 32 segundos, o material destaca os campos nome e e-mail, usa dados fictícios e apresenta o botão de confirmação. Entre 32 e 50 segundos, mostra o resultado da inscrição e a chave mascarada. A orientação em Libras deve expressar que a chave guardada e o e-mail serão usados juntos no próximo acesso. A cartela e a demonstração apoiam a distinção entre o ato de inscrever e o login posterior.

O storyboard de navegação e apoio recebe cerca de 55 segundos. Primeiro localiza horário e recursos na agenda. Depois mostra como mandar uma mensagem privada e como acessar a resposta. Por fim, representa três momentos em sequência: confirmação de presença, emissão e consulta. O último quadro protege os exemplos de credenciais e explica que apenas o código de verificação pode ser compartilhado para consulta.

Os roteiros escritos usam português como referência de sentido e de legenda. Na produção, convém apresentar contexto antes da operação, estabelecer referentes no espaço de sinalização e respeitar expressões não manuais definidas pela pessoa sinalizante. A ordem de um texto escrito pode ser ajustada para uma enunciação compreensível em Libras. A revisão de vocabulário considera o público, o domínio e as variantes de uso.

A documentação de Libras detalha tempos, planos e exemplos de legendas. A aplicação mostra as três sequências em componentes expansíveis acessíveis por teclado. Assim, o avaliador pode consultar o planejamento no mesmo lugar em que os recursos finais seriam disponibilizados.

# 6.3 Produção, análise crítica e validação bilíngue

A pré-produção deve prever imagem da pessoa sinalizante com enquadramento suficiente para mãos, braços, expressões faciais e tronco. Roupas e fundo precisam oferecer contraste. Iluminação uniforme e câmera estável protegem a leitura. Cartelas laterais, legendas e recortes da plataforma devem ocupar espaço que permita observar a sinalização, especialmente em celular. Em demonstrações de dados, e-mails e chaves devem ser fictícios ou mascarados.

O uso de Libras ajuda a ampliar o acesso linguístico, enquanto legendas e texto apoiam pessoas com diferentes preferências. A equipe deve conferir consistência entre português escrito, horário e versão sinalizada. Uma mudança de agenda deve gerar revisão das versões pertinentes. Na comunicação ao vivo, a equipe precisa combinar turnos de fala, visibilidade de intérpretes, forma de enviar perguntas e resposta a interrupções técnicas.

A validação prevista envolve pelo menos três tarefas: encontrar os recursos de uma atividade, efetuar uma inscrição e distinguir chave pessoal de código público. As pessoas avaliam as ações com o material e explicam o que entenderam. A comissão identifica vocabulário confuso, informações difíceis de achar e quadros com excesso de conteúdo. O retorno orienta correções no storyboard e no texto do portal.

O registro de avaliação pode usar observações anonimizadas, com consentimento para eventual gravação, identificação da versão do material e descrição da dificuldade. Uma segunda rodada deve verificar se a mudança resolveu o problema. Para terminologia técnica, consultar especialistas surdos da área ajuda a decidir se convém usar uma variante conhecida, introduzir o termo escrito ou acrescentar demonstração contextual.

A versão entregue contempla o espaço de Libras da interface, o glossário contextual e roteiros de produção. Gravação, revisão de sinais e teste com pessoas surdas estão reservados para a etapa de produção. Dessa forma, os resultados técnicos do portal e a adequação linguística têm evidências próprias.

# 7 RESULTADOS E DISCUSSÃO

## 7.1 Testes de regras de negócio e fluxo HTTP

A compilação do projeto em modo Release foi executada com .NET SDK 10.0.100. A suíte de console validou 11 cenários de negócio e persistência, com zero falhas. Os casos cobriram agenda inicial, filtragem, entradas inválidas, autenticação, e-mail duplicado, condição de presença, persistência, idempotência do certificado, atendimento, encerramento, concorrência, cancelamento e tratamento de armazenamento corrompido.

Em um cenário concorrente, o ambiente tinha apenas uma vaga restante. Oito tentativas foram disparadas por tarefas paralelas. Uma foi aceita. As demais receberam a condição de capacidade preenchida. O resultado confirma a atuação da trava para as requisições do mesmo processo e preserva a capacidade configurada nesse arranjo de avaliação.

O teste de integração HTTP percorreu 20 verificações sem falhas. Foram abertas sessões de participante e comissão, com uma terceira sessão para consultas públicas. O roteiro confirmou recusa de token antifalsificação inválido, proteção de ação administrativa, bloqueio da emissão sem presença, atendimento, aviso público, saída, novo acesso e consulta do certificado por código. Verificou-se carga de cinco horas para a agenda inicial, além de cadastro, alteração de horário e cancelamento de atividade.

A falha de arquivo foi simulada por inserção de JSON inválido. O log registrou a causa de interpretação do arquivo, e a aplicação devolveu uma mensagem de indisponibilidade para a operação. A amostra corrompida foi preservada. Essa distinção entre mensagem operacional e detalhe técnico ajuda a localizar a origem sem expor o diagnóstico ao usuário.

Foram feitas 25 medições de inscrição e de emissão após uma iteração de aquecimento, em arquivo temporário local. As medianas observadas foram de 0,15 ms e 0,27 ms, respectivamente. A medição inclui persistência pelo serviço, com o arquivo e a carga do ambiente avaliado. São valores de demonstração, sujeitos ao sistema de arquivos e ao volume de registros. Um estudo de desempenho para produção precisaria variar carga, concorrência, volume e infraestrutura.

# 7.2 Evidências web, limites e ciclo de melhoria

O roteiro de responsividade verifica sete páginas nas larguras de 320, 390, 768 e 1.440 pixels. Para cada combinação, o teste observa resposta HTTP, existência de um título principal, largura efetiva do documento e associação de rótulos aos controles de formulário. A verificação a 320 pixels também exercita o link de salto por teclado, com transferência de foco para o conteúdo principal. A execução em Chromium aprovou as 28 combinações de página e largura, sem transbordamento horizontal, e a verificação por teclado. As capturas e o procedimento constam no arquivo de evidências do repositório.

A aplicação simula uma edição, com programação inicial de quatro atividades. O evento de demonstração usa salas externas opcionais. A equipe que realizar uma edição deve preparar as salas, confirmar as escalas de interpretação e ensaiar a entrega de legendas. A presença se baseia no registro da comissão e a duração resultante é a soma da agenda ativa no momento de cada emissão, segundo a regra adotada no protótipo.

A integração entre disciplinas ficou evidente no tratamento de uma mudança. Uma atualização na programação altera o estado mantido em C#. O aviso público apresenta a consequência em linguagem direta. O atendimento privado pode acolher solicitações daquele momento. O roteiro bilíngue define como explicar a mudança em um vídeo futuro. As equipes compartilham a mesma informação e conferem os pontos de atualização.

A demonstração opera em processo único e se destina à avaliação local. Para uma implantação institucional, deve-se validar acessibilidade em tecnologias assistivas, estudar segurança operacional e política de dados, avaliar a capacidade de armazenamento e definir responsáveis pelo atendimento. O plano de melhoria inclui registrar tempo de resposta, tipos de barreira, presença e certificação. A comissão usa essas observações para priorizar o próximo ciclo.

Uma avaliação com pessoas surdas é necessária para examinar compreensão dos roteiros, vocabulário sinalizado e execução das tarefas. A comparação entre versões do protótipo deve preservar registro das decisões, teste executado e resultado, para que as mudanças sejam justificáveis.

# 8 CONCLUSÃO

O Conecta TI materializou a proposta de um ambiente virtual responsivo para eventos acadêmicos inclusivos em Tecnologia da Informação. A edição fictícia articula programação, inscrição, comunicação, presença e emissão de certificado. A interface usa recursos do desenvolvimento web responsivo e a aplicação em C# concentra regras, persistência e consultas. Os testes de negócio e de integração confirmaram os fluxos demonstrados, incluindo a condição de presença para emissão e a validação de consulta pública por código.

O plano de comunicação define públicos, mensagens, responsáveis e resposta a incidentes. Os cenários de negociação aplicam escuta ativa, identificação de interesses e critérios comuns, com registro e devolutiva. O conjunto de Libras reúne três storyboards, um glossário contextual e instruções de produção bilíngue. Essas propostas orientam o trabalho da comissão antes, durante e após uma edição.

A solução oferece base para demonstração e discussão acadêmica. A passagem para operação institucional requer validação com pessoas usuárias, revisão linguística, preparação dos recursos anunciados e decisões sobre hospedagem, armazenamento e acesso. A documentação técnica, os testes e os materiais de comunicação permitem reproduzir a simulação e identificar o que deve ser verificado em cada nova edição. O código e as evidências estão no repositório privado do PIM V no GitHub.

# REFERÊNCIAS

BRASIL. Lei nº 10.436, de 24 de abril de 2002. Dispõe sobre a Língua Brasileira de Sinais — Libras e dá outras providências. Brasília, DF: Presidência da República, 2002. Disponível em: https://www.planalto.gov.br/ccivil_03/leis/2002/l10436.htm. Acesso em: 23 nov. 2026.

BRASIL. Decreto nº 5.626, de 22 de dezembro de 2005. Regulamenta a Lei nº 10.436, de 24 de abril de 2002. Brasília, DF: Presidência da República, 2005. Disponível em: https://www.planalto.gov.br/ccivil_03/_ato2004-2006/2005/decreto/d5626.htm. Acesso em: 23 nov. 2026.

FISHER, Roger; URY, William; PATTON, Bruce. Getting to yes: negotiating agreement without giving in. 3. ed. New York: Penguin Books, 2011.

INSTITUTO NACIONAL DE EDUCAÇÃO DE SURDOS (INES). Dicionário Libras. Rio de Janeiro: INES, [s.d.]. Disponível em: https://dicionario.ines.gov.br/. Acesso em: 23 nov. 2026.

MICROSOFT. Desenvolvendo aplicativos MVC do ASP.NET Core. Microsoft Learn, [s.d.]. Disponível em: https://learn.microsoft.com/pt-br/dotnet/architecture/modern-web-apps-azure/develop-asp-net-core-mvc-apps. Acesso em: 23 nov. 2026.

UNIVERSIDADE PAULISTA (UNIP). Manual do PIM V: Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas. [S. l.]: Universidade Paulista, [s.d.]. Material didático fornecido para o projeto.

WORLD WIDE WEB CONSORTIUM (W3C). Web Content Accessibility Guidelines (WCAG) 2.2. W3C Recommendation, 12 dez. 2024. Disponível em: https://www.w3.org/TR/WCAG22/. Acesso em: 23 nov. 2026.
