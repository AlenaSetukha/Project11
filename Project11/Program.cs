using System;
using Lab1;

namespace Test
{
    class Program
    {
        static int Main()
        {
//==============================================1===============================================
            Fv2Complex func = Func.func1;//x * x + y * y
            System.Numerics.Vector2 step = new System.Numerics.Vector2(0.1F, 0.2F);
            V2DataArray x_array = new V2DataArray("Data Array ", DateTime.Now, 3, 2, step, func);
            string format = "F2";
            Console.WriteLine(x_array.ToLongString(format));
            V2DataList x_list = (V2DataList)x_array;
            Console.WriteLine(x_list.ToLongString(format));

            Console.WriteLine("Count in array: {0}, Mindistance in array: {1}\n", 
                    x_array.Count, x_array.MinDistance);
            Console.WriteLine("Count in list: {0}, Mindistance in list: {1}\n",
                    x_list.Count, x_list.MinDistance);

//==============================================2===============================================
            //x_list = x_array(convert), y_list, y_array   
            V2MainCollection x_collect = new V2MainCollection();
            V2DataArray y_array = new V2DataArray("DataArray ", DateTime.Now, 2, 4, step, func);
            V2DataList y_list = new V2DataList("Data_list ", DateTime.Now);
            y_list.AddDefaults(4, func);
            x_collect.Add(x_list);
            x_collect.Add(y_list);
            x_collect.Add(x_array);
            x_collect.Add(y_array);
            Console.WriteLine(x_collect.ToLongString(format));
//==============================================3===============================================
            for (int i = 0; i < x_collect.Count; i++)
            {
                Console.WriteLine("{0} member of collection: Count = {1}, MinDistance = {2}", 
                        i, x_collect[i].Count, x_collect[i].MinDistance);
            }

            return 0;
        }
    }
}

namespace Lab1 
{
    struct DataItem
    {
        public System.Numerics.Vector2 crdnts { get; set; }
        public System.Numerics.Complex field_val { get; set; }
        public DataItem(System.Numerics.Vector2 z, System.Numerics.Complex val)
        {
            field_val = val;
            crdnts = z;
        }
        public string ToLongString(string format)
        {
            return String.Concat(crdnts.X.ToString(format), crdnts.Y.ToString(format),
                    field_val.ToString(format), field_val.Magnitude.ToString(format));
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
//===========================================V2Data=============================================
    abstract class V2Data
    {
        public string ident { get; }
        public DateTime date { get; }
        public V2Data(string s, DateTime t)
        {
            ident = s;
            date = t;
        }
        public abstract int Count { get; }
        public abstract float MinDistance { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return String.Concat(ident, date.ToString());
        }
    }
//=========================================V2DataList===========================================
    class V2DataList : V2Data
    {
        public System.Collections.Generic.List<DataItem> data_list { get; }
        public V2DataList(string s, DateTime t) : base(s, t)
        {
            //распр памяти для list??
            data_list = new System.Collections.Generic.List<DataItem>();
        }
        public bool Add(DataItem newItem)
        {
            foreach (DataItem i in data_list)
            {
                if (i.crdnts == newItem.crdnts)
                {
                    return false;
                }
            }
            data_list.Add(newItem);//library function for add
            return true;
        }

        public int AddDefaults(int nItems, Fv2Complex F)
        {
            int n = 0;//number of added values
            for (int i = 0; i < nItems; i++)
            {
                Random x = new Random();
                double x_cur = Convert.ToDouble(x.Next(-100, 100) / 10.0);
                double y_cur = Convert.ToDouble(x.Next(-100, 100) / 10.0);
                System.Numerics.Vector2 coord = new System.Numerics.Vector2((float)x_cur, (float)y_cur);
                System.Numerics.Complex f_v = F(coord);
                DataItem data_new = new DataItem(coord, f_v);
                if (this.Add(data_new))
                {
                    n++;
                }
            }
            return n;
        }
        public override int Count
        {
            get { return this.data_list.Count; }
        }
        public override float MinDistance
        {
            get
            {
                if (data_list.Count == 1) return 0;
                //min distance between 2 point
                float min = (float)Math.Sqrt((data_list[1].crdnts.X - data_list[0].crdnts.X) *
                                (data_list[1].crdnts.X - data_list[0].crdnts.X) +
                                (data_list[1].crdnts.Y - data_list[0].crdnts.Y) *
                                (data_list[1].crdnts.Y - data_list[0].crdnts.Y));
                for (int i = 0; i < data_list.Count - 1; i++)
                {
                    for (int j = i + 1; j < data_list.Count; j++)
                    {
                        double cur_dist = Math.Sqrt((data_list[i].crdnts.X - data_list[j].crdnts.X) *
                                (data_list[i].crdnts.X - data_list[j].crdnts.X) +
                                (data_list[i].crdnts.Y - data_list[j].crdnts.Y) *
                                (data_list[i].crdnts.Y - data_list[j].crdnts.Y));
                        if ((float)cur_dist < min)
                        {
                            min = (float)cur_dist;
                        }
                    }
                }
                return min;
            }
        }

        public override string ToString()
        {
            return String.Concat("V2DataList ", base.ident, base.date, " Number " +
                "of elements: ", data_list.Count, "\n");
        }
        public override string ToLongString(string format)
        {
            string res = String.Concat(this.ToString());
            foreach (DataItem i in data_list)
            {
                res = String.Concat(res, "Coordinates: ", i.crdnts.ToString(format), " Field value: ",
                        i.field_val.ToString(format), " Field Abs value: ",
                        i.field_val.Magnitude.ToString(format), "\n");
            }
            return res;
        }
    }
//=========================================V2DataArray==========================================
    class V2DataArray : V2Data
    {
        public int X_num_nods { get; }
        public int Y_num_nods { get; }
        public System.Numerics.Vector2 step { get; }
        public System.Numerics.Complex[,] field_val { get; }
        public V2DataArray(string s, DateTime t) : base(s, t)
        {
            field_val = new System.Numerics.Complex[0, 0];
        }
        public V2DataArray(string s1, DateTime t, int x, int y, System.Numerics.Vector2 s,
                Fv2Complex f) : base(s1, t)//f (System.Numerics.Vector2 v2)
        {
            X_num_nods = x;
            Y_num_nods = y;
            step = s;
            field_val = new System.Numerics.Complex[x, y];
            for (int i = 0; i < X_num_nods; i++)
            {
                double x_cur = i * step.X;//follow ox from 0 with step.x step
                for (int j = 0; j < Y_num_nods; j++)
                {
                    double y_cur = j * step.Y;//follow oy from 0 with step.y step
                    System.Numerics.Vector2 v2 = new System.Numerics.Vector2((float)x_cur, (float)y_cur);
                    field_val[i, j] = f(v2);
                }
            }
        }
        public override int Count
        {
            get {
                return X_num_nods * Y_num_nods; }
        }
        public override float MinDistance
        {
            get {
                return (step.X < step.Y ? step.X : step.Y); }
        }

        public override string ToString()
        {
            return String.Concat("V2DataArray ", base.ToString(), " Size: ",  X_num_nods.ToString(),
                    " on ", Y_num_nods.ToString(), ", step in x and y: ", step.ToString() + "\n");
        }
        public override string ToLongString(string format)
        {
            string res = String.Concat(this.ToString());
            for (int i = 0; i < X_num_nods; i++)
            {
                double x_cur = i * step.X;
                for (int j = 0; j < Y_num_nods; j++)
                {
                    double y_cur = j * step.Y;
                    res = res + "Coordinates: x = " + x_cur.ToString(format) + ", y = " + y_cur.ToString(format) +
                            ", Field value =  " + field_val[i, j].ToString(format) + ", Abs Field value = "
                            + field_val[i, j].Magnitude.ToString(format) + "\n";

                }
            }
            return res;
        }

        public static explicit operator V2DataList(V2DataArray a)
        {
            V2DataList res = new V2DataList(a.ident, a.date);
            for (int i = 0; i < a.X_num_nods; i++)
            {
                double x_cur = i * a.step.X;
                for (int j = 0; j < a.Y_num_nods; j++)
                {
                    double y_cur = j * a.step.Y;
                    System.Numerics.Vector2 cord = new System.Numerics.Vector2((float)x_cur, (float)y_cur);
                    DataItem tmp = new DataItem(cord, a.field_val[i,j]);
                    res.Add(tmp);
                }
            }
            return res;
        }
    }
//======================================V2MainCollection========================================
    class V2MainCollection
    {
        private System.Collections.Generic.List<V2Data> data_list = new System.Collections.Generic.List<V2Data>();

        public int Count
        {
            get { return data_list.Count; }
        }
        public V2Data this[int i]
        {
            get { return data_list[i]; }
        }
        public bool Contains(string ID)
        {
            bool res = false;
            foreach (V2Data i in data_list)
            {
                if (i.ident == ID)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }
        public bool Add(V2Data v2data)
        {
            bool res = false;
            if (!data_list.Contains(v2data) || data_list.Count == 0)
            {
                data_list.Add(v2data);
                res = true;
            }
            return res;
        }
        public string ToLongString(string format)
        {
            string res = "V2MainCollection:" + "\n";
            foreach (V2Data i in data_list)
            {
                res += i.ToLongString(format);
            }
            return res;
        }
        public override string ToString()
        {
            string res = "";
            foreach (V2Data i in data_list)
            {
                res += i.ToString();
            }
            return res;
        }
    }
//==========================================Delegate============================================
    public delegate System.Numerics.Complex Fv2Complex(System.Numerics.Vector2 v2);
//==========================================Function============================================
    public static class Func
    {
        public static System.Numerics.Complex func1(System.Numerics.Vector2 v)
        {
            System.Numerics.Complex p = new System.Numerics.Complex(0.0, 1.0);
            System.Numerics.Complex res =  p * (v.X * v.X + v.Y * v.Y);//i(x*x + y*y)
            return res;
        }
    }
}