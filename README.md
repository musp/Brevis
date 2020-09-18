VMBrevisCore
DLL para comunicação á banco de dados pertencente a musp

originario do projeto Brevis-data-manager

Resolve:
Comunicação com o banco de dados via classe com facilidade e clareza, além de ser código aberto, por tanto pode ser customizado conforme a necessidade.
Criado para atender a demanda de migração de software para tecnologias novas.

O que gerou esta demanda:
Ao decorrer do tempo foi visto que existem inúmeras formas de consumir dados em inúmeras empresas, como em vários os casos se trata de empresas que geram portfólio em cima de suas soluções isso gera um legado que muitas vezes não tem compatibilidade e pontos de rastreio de código, este projeto tem o foco de tornar prático o conceito de classe entidade e da segurança de não utilizar código sql em codificação C#.
A integração entre equipes e pessoas tornasse obrigatório, porque gera dependência, tornando as entregas mais assertivas.

Bonus:
Cria dependência forte, ou seja, em uma ramificação muito grande de classes entidade, alterações tendem a quebrar o build com mais facilidade.
Com uma dependência forte, reduz a possibilidade de erros de relacionamento entre tabelas.
Como deve haver clareza, este projeto não realiza os procedimentos de criação de banco, somente manipulação de dados.
É baseado em nomenclatura, por tanto torna o código organizado e rastreável.

Usa Quais tecnologias
Reflection
SqlClient
C#

Ações
Busca de dados
busca.BuscaT(new Empresa() { codigo = usuarioEmpresa.empresaId }, 3);
busca.PesquisaT(new UsuarioEmpresa() { usuarioId = usuarioId }, 3).Where(ue => ue.usuarioId == usuarioId).ToList();
busca.PesquisaTodosT(new Componente(), 10);

Inclusão de dados
inclui.abreTransacao();
inclui.IncluiT(objetoUsuario, 3);
incluir.fechaTransacao();

Altera dados
altera.abreTransacao();
altera.AlteraT(configuracao, 10);
quantas ações forem necessárias*
altera.fechaTransacao();

Objetos
    private string esquema = "TMP"; 
    private int Id ;
    private string Titulo ;
    private int ProjetoId ;
    private bool Ativo ;

    public int codigo
    {
        get { return Id; }
        set { Id = value; }
    }
    public string titulo
    {
        get { return Titulo ; }
        set { Titulo  = value; }
    }
    public int projetoId  {
        get { return ProjetoId; }
        set { ProjetoId = value; }
    }
    public bool ativo
    {
        get { return Ativo; }
        set { Ativo = value; }
    }
    
    public Projeto projeto { get; set; }
    public List<Componente> componentes { get; set; }
    public List<UrlProjetoCss> urlsProjetoCss { get; set; }
Onde as primeiras linhas do código acima representam colunas do banco de dados.(private int Id e demais objetos privados)
Objetos públicos de tipo primitivo e não primitivos, é a representação das colunas da tabela para o destino
Pode ser realizado consulta via string referenciando as classes, com carregamento automático com convenções.

Transparência ao programador e fluidez no aprendizado, mantendo o foco no negócio que é o que realmente deveria importar.

Algumas das definições mostradas acima estão apenas em meu projeto privado, para haver um controle, peço que as pessoas me chamem no e-mail viramundomusp@gmail.com, que eu dou permissão no projeto, dou consultoria, para empresas que queiram migrar de tecnologia ou iniciar um sistema moderno.

Toda manutenção para acesso a banco é facilitado, para garantir performance no desenvolvimento e escalabilidade de carregamento.

Possibilita gerar consultas complexas utilizando linq as consultas e pesquisas padrões, com ou sem o uso do sql, respeitando o modelo de dado retornado.

É sensível às alterações de classes e tabelas, apresentando falha na compilação.

Se adapta a qualquer estrutura de banco, a DLL deve ser configurado para isso.


Como Utilizar

- Inclua este projeto na sua solução VisualStudio
- Existe 2 arquivos xml um para definição das conexões e outro para as ações 
    - XMLConexões - Defina as conexões conforme os bancos que for utlizar.
    - XMLAções - Está configurado para o SqlServer.
- A nomenclatura é necessária, e pode ser alterado os padrões dentro do projet.
- Para este projeto existe apenas as ações de busca funcionais (mais funcionalidades entre em contáto), mas só com isso já dá pra evoluir.
- Vincule o projeto Brevis ao seu projeto de negócio
- Crie uma classe chamada BancoDeDados

using VMBrevisCore.Gerenciador;

namespace XXXXX
{
    public class BancoDeDados
    {
       internal Busca busca = new Busca();
       internal Inclui inclui = new Inclui();
       internal Altera altera = new Altera();
       internal Remove remove = new Remove();
    }
}

 public class EmpresaNegocio : BancoDeDados
{
    public Usuario SalvarUsuario(Usuario objetoUsuario)
    { return inclui.IncluiT<Usuario>(objetoUsuario, 3);}
}
     


