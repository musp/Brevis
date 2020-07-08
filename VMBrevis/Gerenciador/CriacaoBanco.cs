using System.Collections.Generic;
using System.Data.SqlClient;
using VMBrevis.Manipulador;

namespace VMBrevis.Gerenciador
{
    public class CriacaoBanco : Banco
    {
        /// <summary>
        /// Cria banco de dados
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <returns></returns>
        public SqlDataReader CriaBancoT<T>(T dados)
        {
            List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);
            return CriaBanco<object>(parametros, "dbo");
        }
        /// <summary>
        /// Cria tabela no banco conforme dados de entrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <returns></returns>
        public SqlDataReader CriaTabelaT<T>(T dados)
        {
            List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);
            return CriaTabela<object>(parametros, "dbo");
        }
        /// <summary>
        /// Cria relações entre tabelas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dados"></param>
        /// <returns></returns>
        public SqlDataReader CriaChaveT<T>(T dados)
        {
            List<SqlParameter> parametros = MontaParametros(dados, Operacao.Carregar);
            return CriaChave<object>(parametros, "dbo");
        }

    }
}