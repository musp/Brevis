using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using VMBrevis.DadosAcesso;

namespace VMBrevis.Manipulador
{
    public class DadosConexao
    {
        public string nome { get; set; }
        public string stringDeConexao { get; set; }
    }

    public abstract class Dados : Convercoes
    {
        private SqlTransaction transacao;
        private SqlCommand comandoSql;

        public Dados()
        {
            comandoSql = comandoSql == null ? new SqlCommand() : comandoSql;
            comandoSql.Connection = comandoSql.Connection == null ? new SqlConnection() : comandoSql.Connection;
        }

        private SqlDataReader leitorDadosBanco;
        private object retornoInsercao;

        internal virtual void AbreConexao(DadosConexao dadosConexao)
        {
            comandoSql = comandoSql == null ? comandoSql = new SqlCommand() : comandoSql;
            comandoSql.Connection = new SqlConnection(dadosConexao.stringDeConexao.Replace(@"    \",""));
            comandoSql.Connection.Open();
        }

        internal virtual void FechaConexao()
        {
            comandoSql.Connection.Close();
        }

        internal virtual void IniciaTransacao()
        {
            comandoSql.Transaction = comandoSql.Connection.BeginTransaction("Transacao");
        }

        internal virtual void FechaTransacao()
        {
            if (comandoSql.Transaction != null)
                comandoSql.Transaction.Commit();

            if (comandoSql.Connection.State == ConnectionState.Open)
            {
                comandoSql.Connection.Dispose();
            }
        }

        internal virtual void ReverteTransacao()
        {
            comandoSql.Transaction.Rollback();
            comandoSql.Transaction = null;
        }

        internal virtual SqlDataReader CarregaDadosGenericoT<T>(string comando)
        {
            return ExecucaoGenerica<object>(comando);
        }

        //internal virtual SqlDataReader CarregaArquivoBinarioT<T>(T dados, bool listarExcluidos)
        //{
        //    List<SqlParameter> parametros = MontaParametros(dados, Operacao.CarregarBinario, listarExcluidos);
        //    return ExecucaoGenerica<object>(parametros, Esquemas.dbo);
        //}

        //internal virtual SqlDataReader PesquisaDadosExecucaoGenerica<T>(T dados)
        //{
        //    string esquema = dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("esquema")).FirstOrDefault().GetValue(dados).ToString();
        //    List<SqlParameter> parametros = geraParametrosExecucaoProcedure<object>(dados);
        //    return ExecucaoGenerica<object>(parametros, Esquemas.dbo);
        //}

        internal virtual List<SqlParameter> geraParametrosExecucaoProcedure<T>(T dados) where T : class
        {
            FieldInfo[] informacaoCampos = dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.Equals("tabela") && !w.Name.Contains("esquema")).ToArray();
            List<SqlParameter> parametros = new List<SqlParameter>();

            foreach (var item in informacaoCampos)
            {
                if (item.GetValue(dados) != null
                    && (item.FieldType.FullName.Equals(typeof(Int32).ToString()) ? int.Parse(item.GetValue(dados).ToString()) > 0 : true)
                    && (item.GetType().Equals(typeof(DateTime)) ? DateTime.Parse(item.GetValue(dados).ToString()) != DateTime.MinValue : true))
                    parametros.Add(new SqlParameter { ParameterName = "@p_" + item.Name.ToLower(), Value = item.GetValue(dados) });
            }
            return parametros;
        }

        private SqlDataReader ExecucaoGenerica<T>(string comando) where T : class
        {
            comandoSql.Parameters.Clear();

            using (comandoSql)
            {
                if (comandoSql.Connection.State != ConnectionState.Open)
                    comandoSql.Connection.Open();

                comandoSql.CommandText = comando;

                comandoSql.CommandType = CommandType.Text;

                leitorDadosBanco = comandoSql.ExecuteReader();

                return leitorDadosBanco;
            }
        }

        public string RetornaConexao(int conexao)
        {
            switch (conexao)
            {
                case 1: return ConexoesAcessos.ViraMundoBrevis.ToString();
                case 2: return ConexoesAcessos.ViraMundoCertificadoCNJ.ToString();
                case 3: return ConexoesAcessos.ViraMundoDistribuidor.ToString();
                case 4: return ConexoesAcessos.ViraMundoFollowDocs.ToString();
                case 5: return ConexoesAcessos.ViraMundoLogin.ToString();
                case 6: return ConexoesAcessos.ViraMundoSocial.ToString();
                case 7: return ConexoesAcessos.ViraMundoValidacao.ToString();
                case 8: return ConexoesAcessos.ViraMundoLojaVirtual.ToString();
                case 9: return ConexoesAcessos.ViraMundoEventos.ToString();
                default: return "";
            }
        }
    }
}
