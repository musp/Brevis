VMBrevisCore
DLL para comunicação á banco de dados pertencente a musp

originario do projeto Brevis-data-manager

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
Objetos publicos de tipo primitivo e não primitivos, é a representação das colunas da tabela para o destino
Pode ser realizado consulta via string refenciando as classes, com carregamento automático com convenções.

Transparência ao programador e fluidez no aprendizado, mantendo o foco no negócio que é o que realmente deveria importar.

Algumas das definições mostradas acima estão apenas em meu projeto privado, para haver um controle, peço que as pessoas me chamem no email viramundomusp@gmail.com, que eu dou permissão no projeto, dou consultoria, para empresas que queiram migrar de tecnologia ou iniciar um sistema moderno.

Toda manutenção para acesso a banco é facilitado, para gantir performance no desenvolvimento e escalabilidade de carregamento.

Possibilita gerar consultas complexas utilizando linq as consultas e pesquisas padroes, com ou sem o uso do sql, respeitando o modelo de dado retornado.

É senssível as alterações de classes e tabelas, apresentando falha na compilação.

Se adapta a qualquer estrutura de banco, a DLL deve ser configurada para isso.
