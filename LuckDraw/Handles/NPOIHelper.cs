using Microsoft.AspNetCore.Http;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LuckDraw.Handles
{
    public class NPOIHelper
    {
        public static List<T> InputExcel<T>(IFormFile file) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();
                MemoryStream ms = new MemoryStream();//初始化System.IO的一个新实例
                file.CopyTo(ms);//将文件内容复制到ms中
                ms.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(ms);
                ISheet sheet = workbook.GetSheetAt(0);
                IRow celNum = sheet.GetRow(0);
                PropertyInfo[] properties = typeof(T).GetProperties();//获取模型所有字段的类型
                string value = null;
                int num = celNum.LastCellNum;//Excel的列数数
                var n = sheet.LastRowNum;
                for (int i = 1; i <= sheet.LastRowNum; i++)//行数
                {
                    IRow row = sheet.GetRow(i);
                    var obj = new T();
                    for (int j = 0; j < num; j++)
                    {
                        value = row.GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
                        string strGettype = (properties[j+1].PropertyType).FullName;
                        switch (strGettype)
                        {
                            case "System.String":
                                {
                                    properties[j+1].SetValue(obj, value, null);
                                    break;
                                }
                            case "System.DateTime":
                                {
                                    DateTime time = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                                    properties[j+1].SetValue(obj, time, null);
                                    break;
                                }
                            case "System.Boolean":
                                {
                                    bool bol = Convert.ToBoolean(value);
                                    properties[j+1].SetValue(obj, bol, null);
                                    break;
                                }
                            case "System.Int16":
                                {
                                    short int16 = Convert.ToInt16(value);
                                    properties[j+1].SetValue(obj, int16, null);
                                    break;
                                }
                            case "System.Int64":
                                {
                                    long int64 = Convert.ToInt64(value);
                                    properties[j+1].SetValue(obj, int64, null);
                                    break;
                                }
                            case "System.Int32":
                                {
                                    int int32 = Convert.ToInt32(value);
                                    properties[j+1].SetValue(obj, int32, null);
                                    break;
                                }
                            case "System.Byte":
                                {
                                    byte by = Convert.ToByte(value);
                                    properties[j+1].SetValue(obj, by, null);
                                    break;
                                }
                            case "System.Double":
                                {
                                    byte dou = Convert.ToByte(value);
                                    properties[j+1].SetValue(obj, dou, null);
                                    break;
                                }
                            case "System.Decimal":
                                {
                                    decimal dec = Convert.ToDecimal(value);
                                    properties[j+1].SetValue(obj, dec, null);
                                    break;
                                }
                            default:
                                properties[j+1].SetValue(obj, null, null);
                                break;
                        }                      
                    }
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


    }
}
