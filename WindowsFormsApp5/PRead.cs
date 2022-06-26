using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp5
{
    public class PRead
    {
        public PRead(string fn)
        {
            this.fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            this.Init();
            if (fn.ToLower().EndsWith("adult.dat"))
            {
                this.ti.Remove("def/version.txt");
            }
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00003494 File Offset: 0x00001894
        public PRead()
        {
            this.fs.Close();
        }

        // Token: 0x06000038 RID: 56 RVA: 0x000034C8 File Offset: 0x000018C8
        private void Init()
        {
            this.ti = new Dictionary<string, PRead.fe>();
            this.fs.Position = 0L;
            byte[] array = new byte[1024];
            this.fs.Read(array, 0, 1024);
            int num = 0;
            for (int i = 4; i < 255; i++)
            {
                num += BitConverter.ToInt32(array, i * 4);
            }
            byte[] array2 = new byte[16 * num];
            this.fs.Read(array2, 0, array2.Length);
            this.dd(array2, 16 * num, BitConverter.ToUInt32(array, 212));
            int num2 = BitConverter.ToInt32(array2, 12);
            int num3 = num2 - (1024 + 16 * num);
            byte[] array3 = new byte[num3];
            this.fs.Read(array3, 0, array3.Length);
            this.dd(array3, num3, BitConverter.ToUInt32(array, 92));
            int num4 = 0;
            for (int j = 0; j < num; j++)
            {
                int num5 = 16 * j;
                uint l = BitConverter.ToUInt32(array2, num5);
                int num6 = BitConverter.ToInt32(array2, num5 + 4);
                uint k = BitConverter.ToUInt32(array2, num5 + 8);
                uint p = BitConverter.ToUInt32(array2, num5 + 12);
                int m;
                for (m = num6; m < array3.Length; m++)
                {
                    if (array3[m] == 0)
                    {
                        break;
                    }
                }
                string key = Encoding.ASCII.GetString(array3, num4, m - num4).ToLower();
                PRead.fe value = default(PRead.fe);
                value.p = p;
                value.L = l;
                value.k = k;
                this.ti.Add(key, value);
                num4 = m + 1;
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00003670 File Offset: 0x00001A70
        private void gk(byte[] b, uint k0)
        {
            uint num = k0 * 7391u + 42828u;
            uint num2 = num << 17 ^ num;
            for (int i = 0; i < 256; i++)
            {
                num -= k0;
                num += num2;
                num2 = num + 56u;
                num *= (num2 & 239u);
                b[i] = (byte)num;
                num >>= 1;
            }
        }

        // Token: 0x0600003A RID: 58 RVA: 0x000036C8 File Offset: 0x00001AC8
        private void dd(byte[] b, int L, uint k)
        {
            byte[] array = new byte[256];
            this.gk(array, k);
            for (int i = 0; i < L; i++)
            {
                byte b2 = b[i];
                b2 ^= array[i % 253];
                b2 += 3;
                b2 += array[i % 89];
                b2 ^= 153;
                b[i] = b2;
            }
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00003728 File Offset: 0x00001B28
        public byte[] Data(string fn)
        {
            PRead.fe fe;
            if (!this.ti.TryGetValue(fn, out fe))
            {
                return null;
            }
            this.fs.Position = (long)((ulong)fe.p);
            byte[] array = new byte[fe.L];
            this.fs.Read(array, 0, array.Length);
            this.dd(array, array.Length, fe.k);
            return array;
        }

        // Token: 0x04000032 RID: 50
        private FileStream fs;

        // Token: 0x04000033 RID: 51
        public Dictionary<string, PRead.fe> ti;

        // Token: 0x0200000B RID: 11
        public struct fe
        {
            // Token: 0x04000034 RID: 52
            public uint p;

            // Token: 0x04000035 RID: 53
            public uint L;

            // Token: 0x04000036 RID: 54
            public uint k;
        }
    }
}
