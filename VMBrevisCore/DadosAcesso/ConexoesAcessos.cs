using System.ComponentModel;

namespace VMBrevisCore.DadosAcesso
{
    enum ConexoesAcessos
    {
        ViraMundoBrevis = 1,
        ViraMundoCertificadoCNJ = 2,
        ViraMundoDistribuidor = 3,
        ViraMundoFollowDocs = 4,
        ViraMundoLogin = 5,
        ViraMundoSocial = 6,
        ViraMundoValidacao = 7,
        ViraMundoLojaVirtual = 8,
        ViraMundoEventos = 9,
        ViraMundoGeradorDeTela = 10
    }

    enum Esquemas
    {
        [Description("Esquema pricipal")]
        dbo = 1,
        VMP = 2
    }
    enum AcessoProcedure
    {
        [Description("@PRC_")]
        prc = 1
    }
    enum acao
    {
        [Description("SELECT * FROM ")]
        Selecao = 1,
        [Description("UPDATE TABELA SET COLUNAS WHERE CLAUSULAS")]
        Alteracao = 2,
        [Description("INSERT INTO TABELA (COLUNAS) VALUES DADOS")]
        Adicao = 3,
        [Description("DELETE FROM TABELA WHERE CLAUSULAS")]
        RemocaoFisica = 4,
        [Description("UPDATE TABELA SET ativo = 0 WHERE CLAUSULAS")]
        RemocaoLogica = 5
    }
}
