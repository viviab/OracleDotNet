﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Habitania.OracleDbConnection
{
    public static class Extensions
    {

        //TODO: Seria interesante conseguir un mail merger para utlizar plantillas
        //var mandatory = new String[] {"NOMBREOFICINA", "EMAIL"};
        //var values = new { NOMBREOFICINA = nombreOficina, EMAIL = emailTo };
        //var body = @"Muchas gracias por contactar con [#NOMBREOFICINA#] email para: [#EMAIL#]";

        //foreach (var item in mandatory)
        //{
        //    var val = values.GetType().GetProperty(item).GetValue(values).ToString();

        //    body = body.Replace("[#" + item + "#]", val);
        //}

        #region Genericos

        /// <summary>
        /// Evita excepciones de tipo NULLObject al acceder a la propiedad de un objecto que no se ha validado 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static TResult Get<TSource, TResult>(this TSource source, Func<TSource, TResult> selector)
        {
            if (source == null)
                return default(TResult);

            return selector(source);
        }

        public static TResult GetObjectByType<TResult>(this Object[] source)
        {
            if (source == null)
                return default(TResult);

            foreach (var item in source)
            {
                if (item is TResult)
                    return (TResult)item;
            }

            throw new Exception("No existe ningún objeto de tipo " + typeof(TResult));
        }

        public static T TryParseAs<T>(this object input)
        {
            if (typeof(T) == typeof(int?))
                return (T)(object)TryParseAsIntN(input);
            else if (typeof(T) == typeof(int))
                return (T)(object)TryParseAsInt(input);

            else if (typeof(T) == typeof(bool))
                return (T)(object)TryParseAsBool(input);

            else if (typeof(T) == typeof(Decimal))
                return (T)(object)TryParseAsDecimal(input);

            else if (typeof(T) == typeof(Double))
                return (T)(object)TryParseAsDouble(input);
            else if (typeof(T) == typeof(Double?))
                return (T)(object)TryParseAsDoubleN(input);

            else if (typeof(T) == typeof(short))
                return (T)(object)TryParseAsShort(input);
            else if (typeof(T) == typeof(long))
                return (T)(object)TryParseAsLong(input);
            else if (typeof(T) == typeof(float))
                return (T)(object)TryParseAsFloat(input);
            else if (typeof(T) == typeof(float?))
                return (T)(object)TryParseAsFloatN(input);

            else if (typeof(T) == typeof(DateTime))
                return (T)(object)TryParseAsDateTime(input);
            else if (typeof(T) == typeof(DateTime?))
                return (T)(object)TryParseAsDateTimeN(input);

            else if (typeof(T) == typeof(string))
                return (T)(object)TryParseAsString(input);

            return default(T);
        }

        public static int? TryParseAsIntN(this object input)
        {
            if (input == null)
                return default(int?);

            bool value;
            var isBool = bool.TryParse(input.ToString(), out value);
            if (isBool)
                return value ? 1 : 0;

            int num;
            var isNum = int.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return null;
        }

        public static int? TryParseAsIntConvert(this object input)
        {
            if (input == null)
                return default(int?);

            return Convert.ToInt16(input);
        }

        public static int TryParseAsInt(this object input)
        {
            if (input == null)
                return default(int);

            bool value;
            var isBool = bool.TryParse(input.ToString(), out value);
            if (isBool)
                return value ? 1 : 0;

            int num;
            var isNum = int.TryParse(input.ToString(), out num);
            if (isNum)
                return num;

            return default(int);
        }

        public static bool TryParseAsBool(this object input)
        {
            if (input == null)
                return default(bool);

            int num;
            var isNum = int.TryParse(input.ToString(), out num);

            if (isNum)
                return Convert.ToBoolean(num);

            if (input.ToString().IsNullOrEmpty())
                return default(bool);

            return Convert.ToBoolean(input.ToString());
        }

        public static bool? TryParseAsBoolN(this object input)
        {
            if (input == null)
                return default(bool?);

            int num;
            var isNum = int.TryParse(input.ToString(), out num);

            if (isNum)
                return Convert.ToBoolean(num);

            if (input.ToString().IsNullOrEmpty())
                return default(bool?);

            return Convert.ToBoolean(input.ToString());
        }

        public static decimal TryParseAsDecimal(this object input)
        {
            if (input == null)
                return default(Decimal);

            Decimal num;
            var isNum = Decimal.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(Decimal);
        }

        public static float TryParseAsFloat(this object input)
        {
            if (input == null)
                return default(float);

            float num;
            var isNum = float.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(float);
        }

        public static long TryParseAsLong(this object input)
        {
            if (input == null)
                return default(long);

            long num;
            var isNum = long.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(long);
        }

        public static float? TryParseAsFloatN(this object input)
        {
            if (input == null)
                return default(float?);

            float num;
            var isNum = float.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(float?);
        }

        public static double TryParseAsDouble(this object input)
        {
            if (input == null)
                return default(Double);

            Double num;
            var isNum = Double.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(Double);
        }

        public static double? TryParseAsDoubleN(this object input)
        {
            if (input == null)
                return default(Double?);

            Double num;
            var isNum = Double.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(Double?);
        }

        public static short TryParseAsShort(this object input)
        {
            if (input == null)
                return default(short);

            short num;
            var isNum = short.TryParse(input.ToString(), out num);

            if (isNum)
                return num;

            return default(short);
        }

        public static DateTime? TryParseAsDateTimeN(this object input)
        {
            if (input == null)
                return default(DateTime?);

            DateTime date = new DateTime();
            var isDate = DateTime.TryParse(input.ToString(), out date);


            if (date == DateTime.MinValue)
                return null;

            return date;
        }

        public static DateTime TryParseAsDateTime(this object input)
        {
            if (input == null)
                return default(DateTime);

            DateTime date;
            var isDate = DateTime.TryParse(input.ToString(), out date);

            if (isDate)
                return date;

            return default(DateTime);
        }

        public static string TryParseAsString(this object input)
        {
            if (input == null)
                return default(string);

            if (input.ToString() == "null")
                return default(string);

            if (input.ToString() == String.Empty)
                return default(string);

            return input.ToString();
        }

        public static bool Has<T>(this List<T> source, T item)
        {
            if (source == null)
                return false;

            return source.Contains(item);
        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        #endregion

        #region String

        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Envuelve la cadena entre los strings especificados
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string WrapWith(this string value, string start, string end)
        {
            return "{0}{1}{2}".FormatWith(start, value, end);
        }

        /// <summary>
        /// Devuelve true si la string no es nula o vacia
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasValue(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Devuelve true si la string no es nula o vacia
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Simplificacion del metodo String.FormatWith
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string value, params object[] args)
        {
            return String.Format(value, args);
        }

        /// <summary>
        /// Método que devuelve el primer caracter de un String o Empty si es null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetFirstLetter(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return String.Empty;

            return value.FirstOrDefault().ToString();
        }

        /// <summary>
        /// Método que devuelve la cadena con la primera letra en mayúsculas
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UppercaseFirst(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return String.Empty;

            return String.Format("{0}{1}", Char.ToUpper(value[0]), value.Substring(1));
        }

        /// <summary>
        /// Devuelve true si el string contiene alguna de las cadenas especificadas
        /// </summary>
        /// <param name="value"></param>
        /// <param name="patterns"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string value, string[] patterns)
        {
            return patterns.Any(value.Contains);
        }

        /// <summary>
        /// Obtiene el nombre del fichero del blob a traves de su url absoluta y devuelve este normalizado
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NormalizeBlobUri(this string value)
        {
            var result = value.Split('/');
            result[result.Length - 1] = result[result.Length - 1].NormalizeForBlob();

            return String.Join("/", result);
        }

        /// <summary>
        /// Devuelve un string normalizado para el blob
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NormalizeForBlob(this string value)
        {
            var pattern = "[\\`~#%&*<>@?¿¡!()+|:;{}/\\]|\"ýþÿ]";
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            var transformado = regEx.Replace(value, "");
            pattern = @"[âãäåæá]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "a");
            pattern = @"[èéêë]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "e");
            pattern = @"[ìíîï]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "i");
            pattern = @"[òóôõ]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "o");
            pattern = @"[ùúûü]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "u");
            pattern = @"[ñ]";
            regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            transformado = regEx.Replace(transformado, "n");

            transformado = transformado.Replace(" ", "_").Replace("@", "_").ToLower();

            return transformado;
        }

        public static string GetSubstring(this string value, int maxCharacters)
        {
            if (value == null)
                return null;

            if (value.Length > maxCharacters)
                return value.Substring(0, maxCharacters);
            else
                return value;
        }

        public static string SetCDATA(this string value, int maxCharacters)
        {
            if (value == null)
                return null;

            if (value.Length > maxCharacters)
                return value.Substring(0, maxCharacters).SetCDATA();
            else
                return value.SetCDATA();
        }

        public static string SetCDATA(this int? value, int maxCharacters)
        {
            return value.ToString().SetCDATA(maxCharacters);
        }

        public static string SetCDATA(this string value, bool TrimValue)
        {
            return SetCDATA(value.Get(i => i.Trim()));
        }

        public static string SetCDATA(this string value)
        {
            if (value == null)
                return String.Empty;

            return String.Format("<![CDATA[{0}]]>", value);
        }

        public static string SetCDATA(this int? value)
        {
            return value.ToString().SetCDATA();
        }

        public static string SetCDATA(this int value)
        {
            return value.ToString().SetCDATA();
        }

        public static string ToXMLName(this string value)
        {
            var notAllowed = @"\!|#$%&()=?¿¡*[]{};,:<>ç'ºª+""".ToCharArray();

            foreach (var oldChar in notAllowed)
            {
                value = value.Replace(oldChar.ToString(), String.Empty);
            }

            return value.Replace(" ", "_");
        }

        /// <summary>
        /// Devuelve la cadena de texto si se cumple la condicion
        /// </summary>
        /// <param name="value"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static string If(this string value, bool criteria)
        {
            if (criteria)
                return value;

            return String.Empty;
        }

        /// <summary>
        /// Devuelve una string con formato HTML, controlando los line break
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static string GetHTMLString(this string comment)
        {
            if (comment.IsNullOrEmpty())
                return String.Empty;

            comment = comment.Replace("\r\n", "\n").Replace("\n", "<br>");

            return HttpUtility.HtmlDecode(comment);
        }

        public static string convertToDecimalDDBB(this string numberformat)
        {
            return numberformat.Replace(",", ".");
        }

        public static string encodeHTML(this string sCadena)
        {
            sCadena = System.Web.HttpUtility.HtmlEncode(sCadena);
            if (sCadena == null)
                return String.Empty;

            string pattern = @"\r\n?|\n";
            string replacement = "";
            Regex.Replace(sCadena, pattern, replacement);
            sCadena = sCadena.Replace("", "");
            sCadena = sCadena.Replace("’", "'");
            sCadena = sCadena.Replace("€", "&#8364;");
            return sCadena.Trim();
        }

        public static string AddExtensionImage(this string surl)
        {
            if (surl != "" && surl.LastIndexOf(".jpg") == -1)
                surl = "{0}{1}".FormatWith(surl, ".jpg");
            return surl;
        }

        public static string Left(string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }

        public static string Right(string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        #endregion

        #region char

        public static string GetRtfEncoding(this char c)
        {
            if (c == '\\') return "\\\\";
            if (c == '{') return "\\{";
            if (c == '}') return "\\}";
            if (c == '\n') return "\r\n\\line ";
            int intCode = Convert.ToInt32(c);
            if (char.IsLetter(c) && intCode < 0x80)
            {
                return c.ToString();
            }
            return "\\u" + intCode + "?";
        }

        #endregion

        #region bool?

        public static bool IsTrue(this bool? value)
        {
            return (value.HasValue && value.Value);
        }

        public static bool IsFalse(this bool? value)
        {
            return (!value.HasValue || (value.HasValue && !value.Value));
        }

        #endregion

        #region int

        public static bool HasValue(this int? value)
        {
            if (!value.HasValue)
                return false;

            return value.Value.HasValue();
        }

        public static bool HasValue(this int value)
        {
            if (value <= 0)
                return false;

            return true;
        }

        public static bool NotValue(this int? value)
        {
            return !value.HasValue();
        }

        public static bool NotValue(this int value)
        {
            return !value.HasValue();
        }

        public static bool NotValueOrZero(this int? value)
        {
            return (!value.HasValue || value.Value == 0);
        }

        public static bool HasValueNotZero(this int? value)
        {
            return (value.HasValue && value.Value != 0);
        }

        #endregion

        #region DateTime

        public static long GetMilliseconds(this DateTime date)
        {
            var result = (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            return result;
        }

        public static DateTime GetDateTime(this long milliseconds)
        {
            var date = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);

            return date;
        }

        public static bool IsPast(this DateTime date)
        {
            var result = date < DateTime.Now;
            return result;
        }

        public static bool IsFuture(this DateTime date)
        {
            var result = date > DateTime.Now;
            return result;
        }

        public static bool IsToday(this DateTime date)
        {
            if (date.Date == DateTime.Now.Date)
                return true;

            return false;
        }

        public static DateTime DefaultValue(this DateTime date)
        {
            var result = (DateTime.MinValue == date);
            if (result)
                return DateTime.Now;

            return date;
        }

        public static bool HasValueNotMin(this DateTime? value)
        {
            return (value.HasValue && value.Value != DateTime.MinValue);
        }

        #endregion

        #region double?

        public static string ToGeoCoordinate(this double? value)
        {
            return value.ToString().Replace(".", ",");
        }

        #endregion

        #region IDataReader

        public static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        public static T First<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            var result = reader.Select(projection);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Con este metodo evitamos exceptions al acceder a una columna que no exista en el datareader 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetSafe<T>(this IDataReader reader, string columnName)
        {
            var result = GetSafe(reader, columnName);

            return result.TryParseAs<T>();
        }

        public static string GetSafe(this IDataReader reader, string columnName)
        {
            var columnIndex = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(columnIndex))
                return default(string);

            return reader[columnName].ToString();
        }

        #endregion

        #region IEnumerable

        public static void Do<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static bool IsSingle<T>(this IEnumerable<T> enumeration)
        {
            if (enumeration == null)
                return false;

            return enumeration.Count() == 1;
        }

        public static string Join(this IEnumerable<string> enumeration, string separator)
        {
            return String.Join(separator, enumeration.Where(i => i.HasValue()).ToArray());
        }

        public static bool None<T>(this IEnumerable<T> enumeration)
        {
            if (enumeration == null)
                return true;

            return enumeration.Count() == 0;
        }

        public static bool None<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate)
        {
            return enumeration.Where(predicate).Count() == 0;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumeration)
        {
            //if (enumeration.None())
            //    return null;

            return new HashSet<T>(enumeration);
        }

        public static IEnumerable<T> OrderByField<T>(this IEnumerable<T> source, string orderByField, bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";

            var queryableSource = source.AsQueryable<T>();
            var type = typeof(T);
            var field = type.GetField(orderByField);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, field);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, field.FieldType },
                queryableSource.Expression, Expression.Quote(orderByExpression));

            return queryableSource.Provider.CreateQuery<T>(resultExpression);
        }

        #endregion

        #region Datarow

        public static string LeerCampo(this DataRow row, string name)
        {
            try
            {
                return row[name].ToString() != "NULL" ? row[name].ToString().Trim() : null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region XmlDocument

        /// <summary>
        /// Convierte un xml en una clase, solo es valido para xml de un solo nivel, es decir no puede haber nodos anidados
        /// </summary>
        /// <param name="oXML"></param>
        /// <param name="objetosIDE"></param>
        /// <returns></returns>
        public static object GetObjectFrom(this XmlDocument oXML, object objeto)
        {
            foreach (var memberInfo in objeto.GetType().GetMembers().Where(m => m.MemberType == MemberTypes.Property))
            {
                var element = oXML.GetElementsByTagName(memberInfo.Name)[0];
                if (element == null)
                    break;

                var property = objeto.GetType().GetProperty(memberInfo.Name, BindingFlags.Public | BindingFlags.Instance);
                var value = Convert.ChangeType(element.InnerText, property.PropertyType);

                if (property != null && property.CanWrite)
                    property.SetValue(objeto, value, null);

            }
            return objeto;
        }

        public static XmlDocument obtenerXML(String URL)
        {
            XmlDocument oxml = new XmlDocument();
            try
            {
                WebRequest webRequest = WebRequest.Create(URL);

                webRequest.ContentType = "text/xml";
                webRequest.Method = "POST";
                webRequest.Timeout = 300000;

                webRequest.ContentType = "multipart/form-data";
                webRequest.Method = "POST";
                //montamos la cadena para el acortador
                String Parameters = "";
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
                //Calculamos un numero de sg de espera en relacion al tamaño del XML enviado, supondremos un tamaño de 7500 bytes por anuncio, y un tiempo de 6 Sg por anuncio
                if (Parameters.Length > 7500)
                    webRequest.Timeout = (Parameters.Length / 7500) * 6 * 1000;
                else
                    webRequest.Timeout = 6 * 1000;
                webRequest.ContentLength = bytes.Length;

                WebResponse webResponse = webRequest.GetResponse();
                if (webResponse == null)
                    return null;

                Stream receiveStream = webResponse.GetResponseStream();
                System.Text.Encoding encode = System.Text.Encoding.GetEncoding("iso-8859-3");
                StreamReader receiveStreamReader = new StreamReader(receiveStream, encode);
                String stringFileContent = receiveStreamReader.ReadToEnd();
                oxml.LoadXml(stringFileContent);
                receiveStream.Close();

                receiveStreamReader.Close();
                webResponse.Close();
                return oxml;

                /*
             * System.IO.Stream os = webRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = webRequest.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string resultado = sr.ReadToEnd().Trim();
            return resultado;
             */
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Serializer

        /// <summary>
        /// Este metodo devuelve la conversion a XMl, de una clase, pasada metiante serializer
        /// Es necesario un paso previo a la llamada de este metodo 
        /// XmlSerializer serializer = new XmlSerializer(typeof(AdMI)); siendo AdMI la clase que queremos convertir
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="adMI"></param>
        /// <returns></returns>
        public static XmlDocument GetXMLObject(this XmlSerializer serializer, object objclass)
        {
            if (serializer == null)
                return null;

            TextWriter writer = new StreamWriter("conversor.xml");
            serializer.Serialize(writer, objclass);
            writer.Close();
            FileStream fs = new FileStream("conversor.xml", FileMode.Open);
            XmlDocument document = new XmlDocument();
            document.Load(fs);
            return document;
        }

        #endregion

        #region MailMessage

        public static void AddBcc(this MailMessage mail, string emails)
        {
            foreach (var mailAddres in emails.Split(';'))
            {
                mail.Bcc.Add(new MailAddress(mailAddres));
            }
        }

        public static void AddTo(this MailMessage mail, string emails)
        {
            foreach (var mailAddres in emails.Split(';'))
            {
                mail.To.Add(new MailAddress(mailAddres));
            }
        }

        #endregion

        #region Cookies

        /// <summary>
        /// Devuelve un objeto a partir del nombre de una cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static T CookieGet<T>(this HttpRequest request, string cookieName)
        {
            try
            {
                var cookie = request.Cookies.Get(cookieName);
                if (cookie == null)
                    return default(T);

                var value = HttpUtility.UrlDecode(cookie.Value);
                if (value.HasValue())
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    return jss.Deserialize<T>(value);
                }

                return default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        //public static void DeleteCookie(this HttpRequest request, string cookieName)
        //{
        //    var cookie = request.Cookies.Get(cookieName);
        //    if (cookie == null)
        //        return;

        //    HttpCookie myCookie = new HttpCookie(cookieName);
        //    myCookie.Expires = DateTime.Now.AddDays(-1d);
        //    request.Cookies.Add(myCookie);
        //} 

        #endregion

        #region Object

        /// <summary>
        /// Recorre el objeto especificado concatenando en un string el par nombre, valor de las propiedades (primitivas del objeto)
        /// </summary>
        /// <param name="object"></param>
        /// <param name="separator"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string ToQueryString(this object @object, string separator = ",", string mask = "{0}")
        {
            if (@object == null)
                throw new ArgumentNullException("object");

            // Get all properties on the object
            var properties = @object.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.PropertyType.IsPrimitive || x.PropertyType == typeof(string)) //Obtenemos solo propiedades primitivas y String
                .Where(x => x.GetValue(@object, null) != null)
                .Where(x => !String.IsNullOrEmpty(x.GetValue(@object, null).ToString()))
                .ToDictionary(x => x.Name, x => x.GetValue(@object, null));

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(mask.FormatWith(x.Key)), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }

        public static bool AllPropertiesNull(this object @object)
        {
            if (@object == null)
                return true;

            foreach (PropertyInfo propertyInfo in @object.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(@object);
                if (value != null)
                    return false;
            }

            return true;
        }

        public static bool IsNumberDecimal(this object value)
        {
            return value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
        #endregion

        #region HashSet

        public static void AddNotNulls<T>(this HashSet<T> list, T item)
        {
            if (item == null)
                return;

            list.Add(item);
        }

        public static void AddNotNulls<T>(this HashSet<T> list, IEnumerable<T> enumeration)
        {
            foreach (var item in enumeration)
            {
                list.AddNotNulls(item);
            }
        }

        #endregion
    }
}
