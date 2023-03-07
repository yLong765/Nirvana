using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Nirvana
{
    public static class FileUtils
    {
        /// <summary>
        /// 写文件
        /// </summary>
        public static void Write(string path, string value)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(path, FileMode.Create);
                sw = new StreamWriter(fs);
                sw.Write(value);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                sw?.Close();
                fs?.Close();
            }
        }

        /// <summary>
        /// 读文件
        /// </summary>
        public static string Read(string path)
        {
            string result;
            FileStream fs = null;
            StreamReader sr = null;
            try
            {
                fs = new FileStream(path, FileMode.Open);
                sr = new StreamReader(fs);
                result = sr.ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                sr?.Close();
                fs?.Close();
            }

            return result;
        }
    }
}