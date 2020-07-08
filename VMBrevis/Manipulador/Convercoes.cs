using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;

namespace VMBrevis.Manipulador
{
    public enum Operacao
    {
        Inserir = 0,
        Alterar = 1,
        Excluir = 2,
        Carregar = 3,
        CarregarBinario = 4,
        Exclusao = 5
    }
    public abstract class Convercoes
    {
        internal virtual string ConverteDeTParaString<T>(T classe) where T : class
        {
            string valores = String.Empty;
            int indice = 0;
            List<FieldInfo> fields = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.StartsWith("Id") && !w.Name.Equals("tabela") && !w.Name.Contains("proc") && !w.Name.Contains("esquema") && w.CustomAttributes.Count() == 0).ToList();
            fields.AddRange(classe.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.StartsWith("Id")));
            foreach (FieldInfo field in fields)
            {
                if (indice == fields.Count() - 1)
                    valores += (field.GetValue(classe)) == null || (field.FieldType == typeof(DateTime) && field.GetValue(classe).Equals(DateTime.MinValue)) ? "NULL" : (ConverteTypeParaString(field.GetValue(classe))).ToString();
                else
                    valores += (field.GetValue(classe)) == null || (field.FieldType == typeof(DateTime) && field.GetValue(classe).Equals(DateTime.MinValue)) ? "NULL, " : (ConverteTypeParaString(field.GetValue(classe))).ToString() + ", ";
                indice++;
            }
            return valores;
        }
        internal virtual string ConverteTypeParaString(object objetoDeConvercao)
        {
            if (objetoDeConvercao.GetType() == typeof(string))
                return "'" + objetoDeConvercao.ToString() + "'";
            else if (objetoDeConvercao.GetType() == typeof(DateTime))
            {
                DateTime dataHora = DateTime.Parse(objetoDeConvercao.ToString());
                return "'" + dataHora.Year + "-" + (dataHora.Month < 10 ? '0' + dataHora.Month.ToString() : dataHora.Month.ToString()) + "-" + (dataHora.Day < 10 ? '0' + dataHora.Day.ToString() : dataHora.Day.ToString()) + "T" + (dataHora.Hour < 10 ? '0' + dataHora.Hour.ToString() : dataHora.Hour.ToString()) + ":" + (dataHora.Minute < 10 ? '0' + dataHora.Minute.ToString() : dataHora.Minute.ToString()) + ":" + (dataHora.Second < 10 ? '0' + dataHora.Second.ToString() : dataHora.Second.ToString()) + "'";
            }
            else if (objetoDeConvercao.GetType() == typeof(int))
                return objetoDeConvercao.ToString();
            else if (objetoDeConvercao.GetType() == typeof(long))
                return objetoDeConvercao.ToString();
            else if (objetoDeConvercao.GetType() == typeof(Boolean))
                return objetoDeConvercao.ToString() == "False" ? ConverteTypeParaString("0") : ConverteTypeParaString("1");
            else if (objetoDeConvercao.GetType() == typeof(decimal))
                return objetoDeConvercao.ToString();
            else
                return " NULL ";
        }
        internal virtual string CriaExclusaoDados<T>(T dados)
        {
            string camposEValores = string.Empty;
            FieldInfo field = dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("dh_fim_reg")).FirstOrDefault();
            camposEValores += field.Name.ToString() + " = " + ConverteTypeParaString(DateTime.Now);
            return camposEValores;
        }
        internal virtual string CriaAtualizacaoDados<T>(T dados)
        {
            string campos = String.Empty;
            string valores = String.Empty;
            string camposEValores = string.Empty;

            List<FieldInfo> fields = dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.Contains("Id") && !w.Name.Contains("esquema") && !w.Name.Equals("tabela") && !w.Name.Contains("proc") && w.CustomAttributes.Count() == 0).ToList();

            int indice = 0;

            foreach (FieldInfo field in fields)
            {
                if (indice == fields.Count() - 1)
                {
                    campos = field.Name.ToString();
                    valores = (field.GetValue(dados)) == null || field.GetValue(dados).Equals(0) || (field.FieldType == typeof(DateTime) && field.GetValue(dados).Equals(DateTime.MinValue)) ? "NULL" : (ConverteTypeParaString(field.GetValue(dados)));
                    camposEValores += campos + " = " + valores;
                }
                else
                {
                    campos = field.Name.ToString();
                    valores = (field.GetValue(dados)) == null || field.GetValue(dados).Equals(0) || (field.FieldType == typeof(DateTime) && field.GetValue(dados).Equals(DateTime.MinValue)) ? "NULL, " : (ConverteTypeParaString(field.GetValue(dados))) + ", ";
                    camposEValores += campos + " = " + valores;
                }

                indice++;
            }
            return camposEValores;
        }
        private string CriaParametros<T>(List<FieldInfo> campos, T classe) where T : class
        {
            string camposEValores = String.Empty;

            foreach (FieldInfo campo in campos.Where(w => w.CustomAttributes.Count() <= 0))
            {
                if (campo.GetValue(classe) != null && campo.GetValue(classe).ToString() != "0" && campo.GetValue(classe).ToString() != new DateTime().ToString())
                    camposEValores += " and " + campo.Name + " = " +
                        (campo.FieldType == typeof(bool) ?
                            (campo.GetValue(classe).ToString() == "False" ?
                                "0" : "1") :
                                    (ConverteTypeParaString(campo.GetValue(classe))));
                if (campo.GetValue(classe) != null && campo.FieldType == typeof(string))
                    camposEValores += " and " + campo.Name + " = " + ConverteTypeParaString(campo.GetValue(classe));
            }

            return camposEValores;
        }
        internal virtual string RetorneParametroParaBuscaBinaria<T>(T classe) where T : class
        {
            string camposEValores = String.Empty;
            List<FieldInfo> camposChave = classe.GetType().GetRuntimeFields().ToList().Where(w => w.Name.Contains("Id")).ToList();
            camposEValores = CriaParametros(camposChave, classe);
            if (camposEValores == String.Empty)
                throw new System.ArgumentException("Deve ser inserido um valor na classe.");
            return camposEValores;
        }
        internal virtual string RetorneParametrosPorClasse<T>(T classe) where T : class
        {
            string camposEValores = String.Empty;

            List<FieldInfo> campos = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.Contains("esquema") && !w.Name.Equals("tabela") && !w.Name.Contains("proc")).ToList();
            campos.AddRange(classe.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("Id")));
            //Pode ser alterado conforme banco
            //List<FieldInfo> camposChave = campos.Where(w => w.Name.Contains("id_") || w.Name.Contains("cd_")).ToList();

            List<FieldInfo> camposChave = campos.Where(w => w.Name.Contains("Id")).ToList();
            //List<FieldInfo> camposChaveComValor = camposChave.Where(e => e.GetValue(classe) != null && (int)e.GetValue(classe) != 0).Select(s => s).ToList();
            //List<FieldInfo> camposChaveComValor = camposChave.Where(e => e.GetValue(classe) != null && ((e.FieldType == typeof(Int32) && (int)e.GetValue(classe) != 0)) || (e.FieldType == typeof(Int64) && (Int64)e.GetValue(classe) != 0)).Select(s => s).ToList();

            List<FieldInfo> camposChaveComValor = campos.Where(e => e.GetValue(classe) != null).Select(s => s).ToList();
            if (camposChaveComValor.Count > 0)
                camposEValores = CriaParametrosExatos(camposChaveComValor, classe);
            else
                camposEValores = CriaParametros(campos, classe);

            if (camposEValores == String.Empty)
                throw new System.ArgumentException("Deve ser inserido um valor na classe.");

            return camposEValores;
        }

        private string CriaParametrosExatos<T>(List<FieldInfo> campos, T classe) where T : class
        {
            string camposEValores = String.Empty;
            foreach (FieldInfo campo in campos.Where(w => w.CustomAttributes.Count() == 0))
            {
                if (campo.GetValue(classe) != null && !campo.GetValue(classe).Equals(string.Empty) && campo.GetValue(classe).ToString() != "0" && campo.GetValue(classe).ToString() != new DateTime().ToString())
                    camposEValores += " and "
                        + campo.Name + " = "
                        + (ConverteTypeParaString(campo.GetValue(classe)));
            }
            return camposEValores;
        }

        internal virtual List<SqlParameter> MontaParametros<T>(T classe, Operacao tipoProcedure, bool? listarExcluidos = false, object idUsuario = null, bool buscaSemParametro = false)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();
            parametros.Add(new SqlParameter { ParameterName = "tabela", Value = classe.ToString().Split('.').LastOrDefault() });
            parametros.Add(new SqlParameter { ParameterName = "esquema", Value = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("esquema")).FirstOrDefault().GetValue(classe) });
            switch (tipoProcedure)
            {
                case Operacao.Inserir:
                    {
                        parametros.Add(new SqlParameter { ParameterName = "Colunas", Value = RetornaNomesDeMetodosDeTSeparadoPorVirgula<object>(classe) });
                        parametros.Add(new SqlParameter { ParameterName = "Valores", Value = ConverteDeTParaString<object>(classe) });
                        break;
                    }
                case Operacao.Alterar:
                    {
                        parametros.Add(new SqlParameter { ParameterName = "ColunasValores", Value = RetornaNomesDeMetodoseValoresDeTSeparadoPorVirgula<object>(classe) });
                        parametros.Add(new SqlParameter { ParameterName = "Parametros", Value = RetorneParametrosPorClasse<object>(classe) });
                        break;
                    }
                case Operacao.Excluir:
                    {
                        parametros.Add(new SqlParameter { ParameterName = "Operacao", Value = tipoProcedure.ToString() });
                        parametros.Add(new SqlParameter { ParameterName = "Atualizacao", Value = CriaExclusaoDados<object>(classe) });
                        parametros.Add(new SqlParameter { ParameterName = "IdValor", Value = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("Id")).Select(w => w).FirstOrDefault().GetValue(classe) });
                        break;
                    }
                case Operacao.Carregar:
                    {
                        if (!buscaSemParametro)
                        {
                            parametros.Add(new SqlParameter { ParameterName = "Parametros", Value = RetorneParametrosPorClasse<object>(classe) });
                            if (classe.GetType().GetRuntimeFields().ToList().Where(s => s.FieldType != typeof(byte[])).Count() > 0)
                                RetorneStringDeColunasComVirgula(classe, parametros);
                            else
                                parametros.Add(new SqlParameter { ParameterName = "Colunas", Value = SqlString.Null });
                        }
                        break;
                    }
                case Operacao.CarregarBinario:
                    {
                        parametros.Add(new SqlParameter { ParameterName = "Parametros", Value = RetorneParametroParaBuscaBinaria<object>(classe) });
                        if (classe.GetType().GetRuntimeFields().ToList().Where(s => s.FieldType != typeof(byte[])).Count() > 0)
                            RetorneColunaBinaria(classe, parametros);
                        else
                            throw new System.ArgumentException("A classe não tem nenhuma propriedade binaria");
                        break;
                    }
            }

            //if (idUsuario.HasValue)
            //    parametros.AddRange(geraParametrosUsuarioLog<T>(classe, idUsuario.Value));

            return parametros;
        }

        internal virtual string RetornaNomesDeMetodosDeTSeparadoPorVirgula<T>(T classe) where T : class
        {
            string colunas = String.Empty;
            int indice = 0;

            List<FieldInfo> fields = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.StartsWith("Id") && !w.Name.Contains("esquema") && !w.Name.Equals("tabela") && !w.Name.Contains("proc") && w.CustomAttributes.Count() == 0).ToList();
            fields.AddRange(classe.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.StartsWith("Id")));
            foreach (FieldInfo field in fields.Where(w => !w.FieldType.Name.Contains("List")))
            {
                if (indice == fields.Where(w => !w.FieldType.Name.Contains("List")).Count() - 1)
                    colunas += field.Name;
                else
                    colunas += field.Name + ", ";
                indice++;
            }
            return colunas;
        }

        internal virtual string RetornaNomesDeMetodoseValoresDeTSeparadoPorVirgula<T>(T classe) where T : class
        {
            string colunas = String.Empty;
            int indice = 0;

            List<FieldInfo> fields = classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.FieldType != typeof(Nullable<DateTime>) && !w.Name.Contains("Id") && !w.Name.Contains("esquema") && !w.Name.Equals("tabela") && !w.Name.Contains("proc") && w.CustomAttributes.Count() == 0 && w.GetValue(classe) != null).ToList();

            foreach (FieldInfo field in fields.Where(w => !w.FieldType.Name.Contains("List")))
            {
                if (field.FieldType == typeof(string))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = '" + field.GetValue(classe) + "' ";
                    else
                        colunas += field.Name + " = '" + field.GetValue(classe) + "', ";
                }
                else if (field.FieldType == typeof(int))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = " + field.GetValue(classe);
                    else
                        colunas += field.Name + " = " + field.GetValue(classe) + ", ";
                }
                else if (field.FieldType == typeof(Int64))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = " + field.GetValue(classe);
                    else
                        colunas += field.Name + " = " + field.GetValue(classe) + ", ";
                }
                else if (field.FieldType == typeof(bool))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = " + (field.GetValue(classe).ToString() == "true" ? " 1 " : "0");
                    else
                        colunas += field.Name + " = " + (field.GetValue(classe).ToString() == "true" ? " 1 " : "0") + ", ";
                }
                else if (field.FieldType == typeof(DateTime))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = '" + DateTime.Parse(field.GetValue(classe).ToString()).Year + "-" + DateTime.Parse(field.GetValue(classe).ToString()).Month + "-" + DateTime.Parse(field.GetValue(classe).ToString()).Day + " " + DateTime.Parse(field.GetValue(classe).ToString()).Hour + ":" + DateTime.Parse(field.GetValue(classe).ToString()).Minute + "' ";
                    else
                        colunas += field.Name + " = '" + DateTime.Parse(field.GetValue(classe).ToString()).Year + "-" + DateTime.Parse(field.GetValue(classe).ToString()).Month + "-" + DateTime.Parse(field.GetValue(classe).ToString()).Day + " " + DateTime.Parse(field.GetValue(classe).ToString()).Hour + ":" + DateTime.Parse(field.GetValue(classe).ToString()).Minute + "', ";
                }
                else if (field.FieldType == typeof(TimeSpan?))
                {
                    if (indice == fields.Count() - 1)
                        colunas += field.Name + " = '" + field.GetValue(classe).ToString().Substring(0, 5) + "' ";
                    else
                        colunas += field.Name + " = '" + field.GetValue(classe).ToString().Substring(0, 5) + "', ";
                }
                indice++;
            }
            return colunas;
        }

        private void RetorneColunaBinaria<T>(T classe, List<SqlParameter> parametros)
        {
            List<string> colunas = classe.GetType().GetRuntimeFields().ToList().Where(s => s.CustomAttributes.Count() == 0 && s.FieldType == typeof(byte[])).Select(s => s.Name).ToList();
            string colunasComVirgula = string.Empty;
            bool primeiraPosicao = true;
            foreach (string coluna in colunas)
            {
                if (primeiraPosicao)
                {
                    primeiraPosicao = false;
                    colunasComVirgula = coluna;
                }
                else
                {
                    colunasComVirgula += ", " + coluna;
                }
            }
            parametros.Add(new SqlParameter { ParameterName = "@p_coluna", Value = colunasComVirgula });
        }

        internal virtual IEnumerable<SqlParameter> geraParametrosUsuarioLog<T>(T classe, long idUsuario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>();

            parametros.Add(
                new SqlParameter
                {
                    ParameterName = "@p_log",
                    Value = "'" + classe.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.Name.Contains("Id")).Select(x => x.Name).FirstOrDefault() + "', " + idUsuario.ToString(),
                    SqlDbType = SqlDbType.VarChar
                });

            return parametros;
        }

        private void RetorneStringDeColunasComVirgula<T>(T classe, List<SqlParameter> parametros)
        {
            List<string> colunas = classe.GetType().GetRuntimeFields().ToList().Where(s => s.CustomAttributes.Count() == 0 && !s.Name.Equals("esquema") && !s.Name.Equals("tabela") && !s.Name.Equals("proc") && s.FieldType != typeof(byte[])).Select(s => s.Name).ToList();
            string colunasComVirgula = string.Empty;
            bool primeiraPosicao = true;
            foreach (string coluna in colunas)
            {
                if (primeiraPosicao)
                {
                    primeiraPosicao = false;
                    colunasComVirgula = coluna;
                }
                else
                {
                    colunasComVirgula += ", " + coluna;
                }
            }
            parametros.Add(new SqlParameter { ParameterName = "Colunas", Value = colunasComVirgula });
        }

        public List<T> ConverteDeSqlListaDataReaderParaT<T>(SqlDataReader dadosDoBanco)
        {
            T item = default(T);
            List<T> dados = new List<T>();
            using (dadosDoBanco)
            {
                while (dadosDoBanco.Read())
                {
                    item = Activator.CreateInstance<T>();
                    List<FieldInfo> fields = item.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.Equals("tabela") && !w.Name.Contains("proc") && !w.Name.Contains("esquema") && w.CustomAttributes.Count() == 0).ToList();
                    fields.AddRange(item.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    foreach (FieldInfo field in fields)
                    {
                        try
                        {
                            var dadoCarregadoParaName = dadosDoBanco[field.Name];
                            field.SetValue(item, ConverteParaTiposDeClasse(dadoCarregadoParaName, field));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    dados.Add(item);
                }
            }
            return dados;
        }

        private object ConverteParaTiposDeClasse(object objetoDeConvercao, FieldInfo campo)
        {
            Type tipo = objetoDeConvercao.GetType();
            if (!object.Equals(objetoDeConvercao, DBNull.Value))
                if (object.Equals(tipo, typeof(string)) && (campo.Name.Contains("fl_") || campo.Name.Contains("st_")))
                    return objetoDeConvercao.Equals("N") || objetoDeConvercao.Equals("D") ? false : true;
                else
                    return objetoDeConvercao;
            else if (object.Equals(tipo, typeof(DBNull)) && (campo.Name.Contains("fl_") || campo.Name.Contains("st_")))
                return false;
            else if (object.Equals(tipo, typeof(DBNull)) && (campo.ToString().Contains(typeof(DateTime).FullName)))
                return new DateTime();
            else if (object.Equals(tipo, typeof(DBNull)) && (campo.ToString().Contains(typeof(TimeSpan).FullName)))
                return new TimeSpan();
            else if (!object.Equals(tipo, typeof(DBNull)) && object.Equals(tipo, typeof(byte[])) && (campo.ToString().Contains("ob_")))
                return null;
            else if (object.Equals(tipo, typeof(DBNull)) && campo.ToString().Contains(typeof(string).FullName))
                return "";
            else if (object.Equals(tipo, typeof(DBNull)) && typeof(decimal) == campo.FieldType)
                return 0m;
            else if (object.Equals(tipo, typeof(DBNull)) && typeof(double) == campo.FieldType)
                return 0d;
            else if (campo.FieldType == typeof(Nullable<bool>))
                return new Boolean();
            else if (campo.FieldType == typeof(Byte[]))
                return null;
            else if (campo.FieldType == typeof(Nullable<Int64>))
                return null;
            else
                return 0;
        }

        private object ConverteParaTiposDeClasseComBinario(object objetoDeConvercao, FieldInfo campo)
        {
            Type tipo = objetoDeConvercao.GetType();
            if (!object.Equals(tipo, typeof(DBNull)) && object.Equals(tipo, typeof(byte[])))
                return objetoDeConvercao;
            else
                return "";
        }

        internal virtual int ConeverteDeSqlDataReaderParaInt(SqlDataReader dadosDoBanco, int alterados)
        {
            using (dadosDoBanco)
            {
                while (dadosDoBanco.Read())
                {
                    alterados = dadosDoBanco.RecordsAffected;
                }
            }

            return alterados;
        }

        internal virtual T ConverterDeSqlDataReaderParaBinarioEmT<T>(SqlDataReader dadosDoBanco, T dados)
        {
            using (dadosDoBanco)
            {
                while (dadosDoBanco.Read())
                {
                    foreach (FieldInfo field in dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => w.FieldType == typeof(byte[])))
                    {
                        field.SetValue(dados, ConverteParaTiposDeClasseComBinario(dadosDoBanco[field.Name], field));
                    }
                }
            }
            return dados;
        }

        internal virtual T ConverteDeSqlDataReaderParaT<T>(SqlDataReader dadosDoBanco, T dados)
        {
            using (dadosDoBanco)
            {
                while (dadosDoBanco.Read())
                {
                    List<FieldInfo> fields = dados.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(w => !w.Name.Equals("tabela") && !w.Name.Contains("proc") && !w.Name.Contains("esquema") && w.CustomAttributes.Count() == 0).ToList();
                    fields.AddRange(dados.GetType().BaseType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    foreach (FieldInfo field in fields)
                    {
                        try
                        {
                            if (dadosDoBanco.FieldCount > 1)
                            {
                                field.SetValue(dados, ConverteParaTiposDeClasse(dadosDoBanco[field.Name], field));
                            }
                            else
                            {
                                if (field.Name.StartsWith("Id"))
                                {
                                    if (field.FieldType == typeof(long) && !object.Equals(dadosDoBanco[0], DBNull.Value))
                                    {
                                        field.SetValue(dados, Convert.ToInt64(dadosDoBanco[0].ToString()));
                                    }
                                    else if (field.FieldType == typeof(int) && !object.Equals(dadosDoBanco[0], DBNull.Value))
                                    {
                                        field.SetValue(dados, Convert.ToInt32(dadosDoBanco[0].ToString()));
                                    }
                                    else
                                        throw new Exception("Tipo de chave primaria inválida");
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }
                }
            }
            return dados;
        }
    }
}
