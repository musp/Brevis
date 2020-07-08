using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace VMBrevisCore.Manipulador
{
    public class Conexao
    {
        public string nome { get; set; }
        public string stringDeConexao { get; set; }
    }

    public abstract class Banco : Convercoes
    {
        private SqlTransaction transacao;
        private SqlCommand comandoSql;

        public Banco()
        {
            comandoSql = comandoSql == null ? new SqlCommand() : comandoSql;
            comandoSql.Connection = comandoSql.Connection == null ? new SqlConnection() : comandoSql.Connection;
        }

        private SqlDataReader leitorDadosBanco;

        internal virtual void AbreConexao(DadosConexao dadosConexao)
        {
            comandoSql = comandoSql == null ? comandoSql = new SqlCommand() : comandoSql;
            comandoSql.Connection = new SqlConnection(dadosConexao.stringDeConexao);
            comandoSql.Connection.Open();
        }

        internal virtual void FechaConexao()
        {
            comandoSql.Connection.Close();
        }

        internal virtual void IniciaTransacao()
        {
            comandoSql.Transaction = comandoSql.Connection.BeginTransaction("TransacaoGenerica");
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

        internal virtual SqlDataReader CriaBanco<T>(List<SqlParameter> parametros, string esquemaDeBanco) where T : class
        {
            comandoSql.Parameters.Clear();

            using (comandoSql)
            {
                if (comandoSql.Connection.State != ConnectionState.Open)
                    comandoSql.Connection.Open();

                comandoSql.CommandText = esquemaDeBanco + "." + parametros.Where(x => x.ParameterName == "@p_proc").Select(x => x.SqlValue).FirstOrDefault().ToString();

                comandoSql.Parameters.AddRange(parametros.Where(x => x.ParameterName != "@p_proc").Select(x => x).ToArray());

                comandoSql.CommandType = CommandType.StoredProcedure;

                leitorDadosBanco = comandoSql.ExecuteReader();

                return leitorDadosBanco;
            }
        }

        internal virtual SqlDataReader CriaTabela<T>(List<SqlParameter> parametros, string esquemaDeBanco) where T : class
        {
            comandoSql.Parameters.Clear();

            using (comandoSql)
            {
                if (comandoSql.Connection.State != ConnectionState.Open)
                    comandoSql.Connection.Open();

                comandoSql.CommandText = esquemaDeBanco + "." + parametros.Where(x => x.ParameterName == "@p_proc").Select(x => x.SqlValue).FirstOrDefault().ToString();

                comandoSql.Parameters.AddRange(parametros.Where(x => x.ParameterName != "@p_proc").Select(x => x).ToArray());

                comandoSql.CommandType = CommandType.StoredProcedure;

                leitorDadosBanco = comandoSql.ExecuteReader();

                return leitorDadosBanco;
            }
        }

        internal virtual SqlDataReader CriaChave<T>(List<SqlParameter> parametros, string esquemaDeBanco) where T : class
        {
            comandoSql.Parameters.Clear();

            using (comandoSql)
            {
                if (comandoSql.Connection.State != ConnectionState.Open)
                    comandoSql.Connection.Open();

                comandoSql.CommandText = esquemaDeBanco + "." + parametros.Where(x => x.ParameterName == "@p_proc").Select(x => x.SqlValue).FirstOrDefault().ToString();

                comandoSql.Parameters.AddRange(parametros.Where(x => x.ParameterName != "@p_proc").Select(x => x).ToArray());

                comandoSql.CommandType = CommandType.StoredProcedure;

                leitorDadosBanco = comandoSql.ExecuteReader();

                return leitorDadosBanco;
            }
        }
    }
}
