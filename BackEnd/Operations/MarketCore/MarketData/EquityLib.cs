using BhagirathFincareClassLibrary.Models;
using BhagirathFincareUtil;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using BhagirathFincareUtil.Enum;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using log4net;

namespace MarketData
{
    public abstract class EquityLib
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SqlConnection conn;
        string sqlconn;
        SqlDataReader mvrdr = null;
        BhagirathFinCareEntities context = new BhagirathFinCareEntities();
        private void connection()
        {
            try
            {
                //sqlconn = ConfigurationManager.ConnectionStrings["BhagirathFinCare"].ConnectionString;
                //conn = new SqlConnection(sqlconn);
                //conn.Open();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        public object CalculateEquity(DtoRequestModelForCalculate dtoCalculate, out string message)
        {
            Multigainer multiGainer = new Multigainer();
            try
            {
                if (dtoCalculate != null)
                {
                    BhagirathFinCareEntities db = new BhagirathFinCareEntities();
                    DateTime newDateTime = DateTime.Now;
                    DateTime.TryParseExact(dtoCalculate.WorkingDate, "MM/dd/yyyy",
                        new CultureInfo("en-US"),
                        DateTimeStyles.None,
                        out newDateTime);

                    DateTime.TryParseExact(dtoCalculate.ExpiryDate, "MM/dd/yyyy",
                        new CultureInfo("en-US"),
                        DateTimeStyles.None,
                        out newDateTime);

                    multiGainer = new Multigainer()
                    {
                        WorkingDate = newDateTime,
                        txt_m5 = dtoCalculate.Close.ToString(),
                        txt_m4 = dtoCalculate.Open.ToString(),
                        txt_l8 = dtoCalculate.CMP.ToString(),
                        txt_m3 = dtoCalculate.Average.ToString(),
                        txt_t10 = dtoCalculate.IDH.ToString(),
                        txt_d10 = dtoCalculate.IDL.ToString(),
                        expirydate = dtoCalculate.ExpiryDate,
                        SelectSymbol = dtoCalculate.Symbole,
                        strkeprice = dtoCalculate.StrickPrice,
                    };

                    try
                    {
                        string Companyname = string.Empty;

                        if (dtoCalculate.Exchange.Equals("BSE", System.StringComparison.OrdinalIgnoreCase))
                        {
                            Companyname = GetSymbolNameFromScriptCode(dtoCalculate.Symbole);
                            multiGainer.Companyname = Companyname;
                        }
                        else
                        {
                            Companyname = dtoCalculate.Symbole;
                            multiGainer.Companyname = Companyname;
                        }

                        double strikeprice = 0;
                        if (multiGainer.strikeprice != null)
                        {
                            strikeprice = Convert.ToDouble(multiGainer.strikeprice.ToString());
                        }
                        string expirydate = "";
                        if (multiGainer.expirydate != null)
                        {
                            expirydate = multiGainer.expirydate.ToString();
                        }

                        string Exchange = dtoCalculate.Exchange;
                        string type = dtoCalculate.Type;
                        double IDH = Convert.ToDouble(dtoCalculate.IDH);
                        double idl = Convert.ToDouble(dtoCalculate.IDL);

                        FileTypeEnum fileType = FileTypeEnum.BSEEQ;

                        switch (Exchange.ToLower())
                        {
                            case "bse":
                                if (type.Equals("EQ", StringComparison.OrdinalIgnoreCase))
                                    fileType = FileTypeEnum.BSEEQ;
                                else if (type.Equals("DERIVATIVE", StringComparison.OrdinalIgnoreCase))
                                    fileType = FileTypeEnum.BSEDERIVATIVE;
                                break;
                            case "nse":
                                if (type.Equals("EQ", StringComparison.OrdinalIgnoreCase))
                                    fileType = FileTypeEnum.NSEEQ;
                                else if (type.Equals("DERIVATIVE", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (dtoCalculate.Instrument.Equals("FUTIDX", StringComparison.OrdinalIgnoreCase) || dtoCalculate.Instrument.Equals("FUTIVX", StringComparison.OrdinalIgnoreCase) || dtoCalculate.Instrument.Equals("FUTSTK", StringComparison.OrdinalIgnoreCase))
                                        fileType = FileTypeEnum.NSEDERIVATIVE;
                                    else
                                        fileType= FileTypeEnum.NSEDERIVATIVEOPT;
                                }
                                break;
                        }

                        connection();

                        if (fileType == FileTypeEnum.NSEEQ)
                        {
                            var time = DateTime.Now;
                            if (time.Hour < 9 || (time.Hour < 9 && time.Minute < 45))
                            {
                                multiGainer.TOTALTRADEDVALUE = context.RecursiveUpload.Where(x => x.FileType == 4 && x.UploadDateTime == DateTime.Today && x.Symbol == Companyname).OrderBy(x => x.FileNo).Select(x => x.TotalTradeValue).Distinct().FirstOrDefault();
                                multiGainer.TOTTRDVAL = context.MarketPrice.Where(x => x.FileType == 4 && x.UploadDate == DateTime.Today && x.Symbol == Companyname).Select(x => x.TotalTradeValue).Distinct().FirstOrDefault();

                            }

                        }
                        var pdlpdhData = GetPDLPDHData(dtoCalculate);
                        if (pdlpdhData != null)
                        {
                            multiGainer.txt_d11 = pdlpdhData.PDL.ToString();
                            multiGainer.txt_t11 = pdlpdhData.PDH.ToString();

                            var tdltdhData = GetTDLTDHData(dtoCalculate, pdlpdhData.PDL, pdlpdhData.PDH, pdlpdhData.SelectedDateForPDHAndPDL, Companyname);
                            if (tdltdhData != null)
                            {
                                multiGainer.txt_d8 = tdltdhData.TDL.ToString();
                                multiGainer.txt_t8 = tdltdhData.TDH.ToString();
                            }

                            var cdlcdhData = GetCTLDCTHDData(dtoCalculate, (decimal)idl, (decimal)IDH, DateTime.Parse(dtoCalculate.WorkingDate), Companyname);

                            if (cdlcdhData != null)
                            {
                                multiGainer.txt_f19 = cdlcdhData.CTLD.ToString();
                                multiGainer.txt_q16 = cdlcdhData.CTHD.ToString();
                            }
                        }
                        else
                        {
                            multiGainer.txt_d11 = 0.ToString();
                            multiGainer.txt_t11 = 0.ToString();
                            multiGainer.txt_d8 = 0.ToString();
                            multiGainer.txt_t8 = 0.ToString();
                            multiGainer.txt_f19 = 0.ToString();
                            multiGainer.txt_q16 = 0.ToString();

                        }


                    }
                    finally
                    {
                    }

                    //
                    #region variable
                    // this variable is for the first row of the calculation 
                    // fix value for calculation 
                    double k2, j2, i2, h2, g2, f2, e2, d2;
                    double n2, o2, p2, q2, r2, s2, t2, u2;
                    // this variable is declared inplace of ws1....ws24 variaable 
                    double tempws1, tempws2, tempws3, tempws4, tempws5, tempws6, tempws7, tempws8, tempws9, tempws10, tempws11, tempws12;
                    double tempws13, tempws14, tempws15, tempws16, tempws17, tempws18, tempws19, tempws20, tempws21, tempws22, tempws23, tempws24;

                    // this variable for wr values
                    double tempwr1, tempwr2, tempwr3, tempwr4, tempwr5, tempwr6, tempwr7, tempwr8, tempwr9, tempwr10, tempwr11, tempwr12;
                    double tempwr13, tempwr14, tempwr15, tempwr16, tempwr17, tempwr18, tempwr19, tempwr20, tempwr21, tempwr22, tempwr23, tempwr24;
                    // variable for calculation for d3, d4 .....u3...u6
                    double c2, c3, c4, c5, c6;
                    double d3, d4, d5, d6;
                    double e3, e4, e5, e6;
                    double f3, f4, f5, f6;
                    double g3, g4, g5, g6;
                    double h3, h4, h5, h6;
                    double i3, i4, i5, i6;
                    double j3, j4, j5, j6;
                    double k3, k4, k5, k6;
                    double m3, m4, m5, m6;
                    double n3, n4, n5, n6;
                    double o3, o4, o5, o6;
                    double p3, p4, p5, p6;
                    double q3, q4, q5, q6;
                    double r3, r4, r5, r6;
                    double s3, s4, s5, s6;
                    double t3, t4, t5, t6;
                    double u3, u4, u5, u6;

                    string a7, b7, c7, d7, e7, f7, g7;
                    double h7, i7, k7;
                    double m7, n7, p7;
                    string q7, r7, s7, t7, u7;

                    // Tdl row variable
                    double d8;
                    string e8, f8, g8, h8, i8;
                    double k8, l8, m8;
                    string q8, r8, s8, n8, p8;
                    double t8;

                    double c9, d9;
                    string e9, f9, g9;
                    double k9, m9;
                    string q9, r9, s9, l9, n9, h9, i9, p9;
                    double t9, u9;

                    double d10, f10, h10, i10, j10, k10, m10, o10, q10, p10, t10, n10;
                    string s10, l10, c10, u10;

                    double c11, d11, r11, f11;
                    string j11, k11, m11, n11, s11, g11, o11, u11;
                    double t11;

                    double a12, b12, c12, d12, e12, f12, g12, i12, n12, o12, s12, t12, j12, q12;
                    string h12, p12, k12;

                    double a13, b13, g13, h13, j13, m13, n13;
                    string i13, k13, o13;

                    // row number 14 is not required
                    // row no 15 is not declared
                    double r15;

                    double a16, b16, c16, d16, e16, f16, g16, h16, i16, j16, k16, l16, m16, n16, o16, p16, q16;
                    string r16;
                    // row number 17 is not required

                    // row 18
                    double e18;


                    // row 19

                    double f19, g19, h19, i19, j19, k19, l19, m19, n19, o19, p19, q19, r19, s19, t19;
                    string e19;
                    // row 20

                    double b20, d20, e20, f20, g20, h20, i20, j20, k20, l20, m20, n20, o20, p20, q20, r20, s20, t20;

                    // row 21

                    double b21, d21, e21, f21, g21, h21, i21, j21, k21, l21, m21, n21, o21, p21, q21, r21, s21, t21;

                    // row 22
                    double c22, d22;
                    //row 23

                    double j23;

                    double m23, m24, n23, n24;
                    // row 24

                    string i24, j24;
                    double h24, d24;

                    // row 25

                    double i25, j25;

                    double[] arrws = new double[16];
                    double[] arrwr = new double[14];

                    // following variable used for recalculation option
                    bool goforrecaculation = false;
                    // special case flag where it is special case no need for reverse call and bandview call as perr discussion with suniljoshi on 9/may/2015 on morning
                    bool specialcase = false;

                    // flag to display trandview or not
                    bool showtrandview = true;

                    int buycount = 0;
                    int salcount = 0;

                    // clearing variables for reverse call
                    // multiGainer.txt_possibilities = "";
                    multiGainer.txt_reversecall = "";
                    multiGainer.Txt_salesl = "";
                    multiGainer.Txt_salet1 = "";
                    multiGainer.Txt_salet2 = "";
                    multiGainer.Txt_salet3 = "";

                    multiGainer.txt_Buysl = "";
                    multiGainer.txt_buyt1 = "";
                    multiGainer.txt_buyt2 = "";
                    multiGainer.Txt_buyt3 = "";

                    m3 = Convert.ToDouble(multiGainer.txt_m3); //3188; //Average
                    m4 = Convert.ToDouble(multiGainer.txt_m4);  //open
                    m5 = Convert.ToDouble(multiGainer.txt_m5);  //c
                    l8 = Convert.ToDouble(multiGainer.txt_l8);  //cmp
                    d8 = Convert.ToDouble(multiGainer.txt_d8);  //tdl
                    t8 = Convert.ToDouble(multiGainer.txt_t8);  //tdh
                    t10 = Convert.ToDouble(multiGainer.txt_t10); //idh
                    d10 = Convert.ToDouble(multiGainer.txt_d10); //idl
                    t11 = Convert.ToDouble(multiGainer.txt_t11); //pdh
                    d11 = Convert.ToDouble(multiGainer.txt_d11);//pdl
                    q16 = Convert.ToDouble(multiGainer.txt_q16);//CTHD
                    f19 = Convert.ToDouble(multiGainer.txt_f19);//ctld

                    k13 = "";
                    #endregion
                    b12 = (t11 - d11) * 100 / m3;
                    // b12 =  Math.Truncate(100 * b12) / 100;  commented on 24april2015

                    //b12 = Math.Round(b12, 2); // commented here on 21/may/2015
                    // b13 = (t8-d8)*100 / m5; // commented on 17april2015
                    b13 = (t8 - d8) * 100 / m3;
                    // b13 = Math.Truncate(100 * b13) / 100;  commented on 24/april/2015

                    //b13 = Math.Round(b13, 2); // commented here on 21/may/2015

                    a13 = (b12 > b13 ? (b12 - b13) * 0.5 : ((b13 - b12) * 0.5));
                    //a13 = Math.Truncate(100 * a13) / 100;  // commented on 24/april/2015


                    //a13 = Math.Round(a13, 2); // commented here on 21/may/2015


                    #region 2row
                    // calculation for s1..s8

                    d2 = 0.0377;
                    e2 = 0.0233;
                    f2 = 0.0144;
                    g2 = 0.0089;
                    h2 = 0.00618;
                    i2 = 0.00382;
                    j2 = 0.0021;
                    k2 = 0.0013;

                    n2 = 0.0013;
                    o2 = 0.0021;
                    p2 = 0.00382;
                    q2 = 0.00618;
                    r2 = 0.0089;
                    s2 = 0.0144;
                    t2 = 0.0233;
                    u2 = 0.0377;
                #endregion
                #region 3row

                // start recalcultion from here 
                // controll is moved back from reverse call 
                // for making m3 average value equal to m6 value on 5/may/2015
                Recalculate:
                    // row 3 calculation

                    //=IF(B12<B13,B12+A13,B13+A13)

                    c2 = (b12 < b13 ? (b12 + a13) : (b13 + a13));
                    //  double test = Math.Truncate(100 * c2) / 100;
                    //c2 = Math.Round(c2, 2); // commented here on 21/may/2015
                    c2 = Math.Round(c2, 6);

                    // c2 = 3.48;
                    // m3 = 3188;

                    //c3 = Math.Round((c2 * 1),2);// commented on 21/may/2015
                    c3 = (c2 * 1);
                    d3 = (m3 - (m3 * d2 * c3));
                    // d3 = ((m3 * d2 * c3)-m3);
                    //  d3 = m3 - (m3 * d2 * c3);
                    //    d3 = (m3 * d2 * c3) - m3;
                    d3 = Math.Round(d3, 2);

                    e3 = (m3 - (m3 * e2 * c3));
                    e3 = Math.Round(e3, 2);

                    f3 = (m3 - (m3 * f2 * c3));
                    f3 = Math.Round(f3, 2);

                    g3 = (m3 - (m3 * g2 * c3));
                    g3 = Math.Round(g3, 2);

                    h3 = (m3 - (m3 * h2 * c3));
                    h3 = Math.Round(h3, 2);

                    i3 = (m3 - (m3 * i2 * c3));
                    i3 = Math.Round(i3, 2);

                    j3 = (m3 - (m3 * j2 * c3));
                    j3 = Math.Round(j3, 2);


                    k3 = (m3 - (m3 * k2 * c3));
                    k3 = Math.Round(k3, 2);

                    n3 = (m3 + (m3 * n2 * c3));
                    n3 = Math.Round(n3, 2);


                    o3 = (m3 + (m3 * o2 * c3));
                    o3 = Math.Round(o3, 2);

                    p3 = (m3 + (m3 * p2 * c3));
                    p3 = Math.Round(p3, 2);

                    q3 = (m3 + (m3 * q2 * c3));
                    q3 = Math.Round(q3, 2);

                    r3 = (m3 + (m3 * r2 * c3));
                    r3 = Math.Round(r3, 2);

                    s3 = (m3 + (m3 * s2 * c3));
                    s3 = Math.Round(s3, 2);

                    t3 = (m3 + (m3 * t2 * c3));
                    t3 = Math.Round(t3, 2);

                    u3 = (m3 + (m3 * u2 * c3));
                    u3 = Math.Round(u3, 2);

                    #endregion
                    #region 4row
                    // row 4 calculation

                    // m4 = 3196;
                    c4 = (c2 * 1);
                    //c4 = Math.Round(c4, 2); commented on 21/may/2015

                    d4 = (m4 - (m4 * d2 * c4));
                    d4 = Math.Round(d4, 2);

                    e4 = (m4 - (m4 * e2 * c4));
                    e4 = Math.Round(e4, 2);

                    f4 = (m4 - (m4 * f2 * c4));
                    f4 = Math.Round(f4, 2);

                    g4 = (m4 - (m4 * g2 * c4));
                    g4 = Math.Round(g4, 2);

                    h4 = (m4 - (m4 * h2 * c4));
                    h4 = Math.Round(h4, 2);

                    i4 = (m4 - (m4 * i2 * c4));
                    i4 = Math.Round(i4, 2);

                    j4 = (m4 - (m4 * j2 * c4));
                    j4 = Math.Round(j4, 2);

                    k4 = (m4 - (m4 * k2 * c4));
                    k4 = Math.Round(k4, 2);

                    n4 = (m4 + (m4 * n2 * c4));
                    n4 = Math.Round(n4, 2);

                    o4 = (m4 + (m4 * o2 * c4));
                    o4 = Math.Round(o4, 2);


                    p4 = (m4 + (m4 * p2 * c4));
                    p4 = Math.Round(p4, 2);

                    q4 = (m4 + (m4 * q2 * c4));
                    q4 = Math.Round(q4, 2);

                    r4 = (m4 + (m4 * r2 * c4));
                    r4 = Math.Round(r4, 2);

                    s4 = (m4 + (m4 * s2 * c4));
                    s4 = Math.Round(s4, 2);

                    t4 = (m4 + (m4 * t2 * c4));
                    t4 = Math.Round(t4, 2);

                    u4 = (m4 + (m4 * u2 * c4));
                    u4 = Math.Round(u4, 2);
                    #endregion
                    #region 5row
                    // row 5 calculation

                    //  m5 = 3163;
                    c5 = ((t11 - d11) * 100 / m5);
                    //c5 = Math.Round(c5, 2); commented here on 21/may/2015
                    c5 = Math.Round(c5, 6);


                    d5 = (m5 - (m5 * d2 * c5));
                    d5 = Math.Round(d5, 2);

                    e5 = (m5 - (m5 * e2 * c5));
                    e5 = Math.Round(e5, 2);

                    f5 = (m5 - (m5 * f2 * c5));
                    f5 = Math.Round(f5, 2);

                    g5 = (m5 - (m5 * g2 * c5));
                    g5 = Math.Round(g5, 2);

                    h5 = (m5 - (m5 * h2 * c5));
                    h5 = Math.Round(h5, 2);

                    i5 = (m5 - (m5 * i2 * c5));
                    i5 = Math.Round(i5, 2);

                    j5 = (m5 - (m5 * j2 * c5));
                    j5 = Math.Round(j5, 2);

                    k5 = (m5 - (m5 * k2 * c5));
                    k5 = Math.Round(k5, 2);

                    n5 = (m5 + (m5 * n2 * c5));
                    n5 = Math.Round(n5, 2);


                    o5 = (m5 + (m5 * o2 * c5));
                    o5 = Math.Round(o5, 2);

                    p5 = (m5 + (m5 * p2 * c5));
                    p5 = Math.Round(p5, 2);

                    q5 = (m5 + (m5 * q2 * c5));
                    q5 = Math.Round(q5, 2);

                    r5 = (m5 + (m5 * r2 * c5));
                    r5 = Math.Round(r5, 2);

                    s5 = (m5 + (m5 * s2 * c5));
                    s5 = Math.Round(s5, 2);


                    t5 = (m5 + (m5 * t2 * c5));
                    t5 = Math.Round(t5, 2);

                    u5 = (m5 + (m5 * u2 * c5));
                    u5 = Math.Round(u5, 2);
                    #endregion
                    #region 6row
                    // row 6 calculation
                    c6 = (c5 / c2);
                    // c6 = Math.Round(c6, 2); commented here on 21/may/2015
                    c6 = Math.Round(c6, 6);

                    d6 = ((d4 + d5 + d3) / 3);
                    d6 = Math.Round(d6, 2);

                    e6 = ((e4 + e5 + e3) / 3);
                    e6 = Math.Round(e6, 2);

                    f6 = ((f4 + f5 + f3) / 3);
                    f6 = Math.Round(f6, 2);

                    g6 = ((g4 + g5 + g3) / 3);
                    g6 = Math.Round(g6, 2);

                    h6 = ((h4 + h5 + h3) / 3);
                    h6 = Math.Round(h6, 2);

                    i6 = ((i4 + i5 + i3) / 3);
                    i6 = Math.Round(i6, 2);

                    j6 = ((j4 + j5 + j3) / 3);
                    j6 = Math.Round(j6, 2);

                    k6 = ((k4 + k5 + k3) / 3);
                    k6 = Math.Round(k6, 2);

                    // start recalcultion from here 
                    // controll is moved back from reverse call 
                    // for making m3 average value equal to m6 value on 2/may/2015
                    // Recalculate:
                    m6 = ((m4 + m5 + m3) / 3);
                    m6 = Math.Round(m6, 2);


                    n6 = ((n4 + n5 + n3) / 3);
                    n6 = Math.Round(n6, 2);


                    o6 = ((o4 + o5 + o3) / 3);
                    o6 = Math.Round(o6, 2);

                    p6 = ((p4 + p5 + p3) / 3);
                    p6 = Math.Round(p6, 2);

                    q6 = ((q4 + q5 + q3) / 3);
                    q6 = Math.Round(q6, 2);

                    r6 = ((r4 + r5 + r3) / 3);
                    r6 = Math.Round(r6, 2);

                    s6 = ((s4 + s5 + s3) / 3);
                    s6 = Math.Round(s6, 2);

                    t6 = ((t4 + t5 + t3) / 3);
                    t6 = Math.Round(t6, 2);

                    u6 = ((u4 + u5 + u3) / 3);
                    u6 = Math.Round(u6, 2);

                    #endregion

                    #region 18row
                    //row 18

                    //e18
                    //=IF(L8>F19,L8-F19,F19-L8)

                    e18 = (l8 > f19 ? (l8 - f19) : (f19 - l8));
                    e18 = Math.Round(e18, 2);


                    tempwr1 = 0.00382;
                    tempwr2 = 0.00618;
                    tempwr3 = 0.0089;
                    tempwr4 = 0.0144;

                    tempwr6 = Math.Round((tempwr3 + tempwr4), 4);
                    tempwr8 = Math.Round((tempwr4 + tempwr6), 4);
                    tempwr10 = Math.Round((tempwr6 + tempwr8), 4);
                    tempwr12 = Math.Round((tempwr8 + tempwr10), 4);
                    tempwr14 = Math.Round((tempwr10 + tempwr12), 4);
                    tempwr16 = Math.Round((tempwr12 + tempwr14), 4);
                    tempwr18 = Math.Round((tempwr14 + tempwr16), 4);
                    tempwr20 = Math.Round((tempwr16 + tempwr18), 4);
                    tempwr22 = Math.Round((tempwr18 + tempwr20), 4);
                    tempwr24 = Math.Round((tempwr20 + tempwr22), 4);

                    // wr value is not calculated here if required then calculate here
                    #endregion
                    #region 19row
                    // row 19 calculation


                    //CTLD Calculation

                    e19 = "CTLD";
                    // commented on 11april2015 need to calculate later from database   f19 = 3061; // this level needs to be find out by logic



                    //g19
                    //=SUM(F19+(F19*P2*C2))

                    g19 = (f19 + (f19 * p2 * c2));
                    g19 = Math.Round(g19, 2);
                    arrwr[0] = g19;
                    //h19
                    //=SUM(F19+(F19*Q2*C2))

                    h19 = (f19 + (f19 * q2 * c2));
                    h19 = Math.Round(h19, 2);
                    arrwr[1] = h19;
                    //i19
                    //=SUM(F19+(F19*R2*C2))

                    i19 = (f19 + (f19 * r2 * c2));
                    i19 = Math.Round(i19, 2);
                    arrwr[2] = i19;
                    //j19
                    //=SUM(F19+(F19*S2*C2))

                    j19 = (f19 + (f19 * s2 * c2));
                    j19 = Math.Round(j19, 2);
                    arrwr[3] = j19;

                    //l19

                    //=SUM(F19+(F19*T2*C2))

                    l19 = (f19 + (f19 * t2 * c2));
                    l19 = Math.Round(l19, 2);
                    arrwr[4] = l19;

                    //n19
                    //=SUM(F19+(F19*U2*C2))

                    n19 = (f19 + (f19 * u2 * c2));
                    n19 = Math.Round(n19, 2);
                    arrwr[5] = n19;

                    //p19
                    //=SUM(F19+(F19*P18*C2))

                    p19 = (f19 + (f19 * tempwr10 * c2)); /// need to calculate the value for wr10
                    p19 = Math.Round(p19, 2);
                    arrwr[6] = p19;


                    //r19
                    //=SUM(F19+(F19*R18*C2))

                    r19 = (f19 + (f19 * tempwr12 * c2)); // need to calculate value for wr12
                    r19 = Math.Round(r19, 2);
                    arrwr[7] = r19;

                    //t19

                    //=SUM(F19+(F19*T18*C2))

                    t19 = (f19 + (f19 * tempwr14 * c2));  // need to calculate value for wr14
                    t19 = Math.Round(t19, 2);
                    arrwr[8] = t19;
                    #endregion
                    #region 15row
                    // row 15 calculation

                    tempws1 = 0.00382;
                    tempws2 = 0.00618;
                    tempws3 = 0.0089;
                    tempws4 = 0.0144;

                    tempws6 = Math.Round((tempws3 + tempws4), 4);
                    tempws8 = Math.Round((tempws4 + tempws6), 4);
                    tempws10 = Math.Round((tempws6 + tempws8), 4);
                    tempws12 = Math.Round((tempws8 + tempws10), 4);
                    tempws14 = Math.Round((tempws10 + tempws12), 4);
                    tempws16 = Math.Round((tempws12 + tempws14), 4);
                    tempws18 = Math.Round((tempws14 + tempws16), 4);
                    tempws20 = Math.Round((tempws16 + tempws18), 4);
                    tempws22 = Math.Round((tempws18 + tempws20), 4);
                    tempws24 = Math.Round((tempws20 + tempws22), 4);

                    //r15
                    //=IF(Q16>L8,Q16-L8,L8-Q16)

                    r15 = Math.Round((q16 > l8 ? (q16 - l8) : (l8 - q16)), 2);
                    #endregion

                    #region 16row
                    //Row 16 calculation for CTHD 
                    r16 = "CTHD";
                    //q16 = 3295;  // this level needs to be find using logic
                    //p16
                    //=SUM(Q16-(Q16*I2*C2))
                    p16 = (q16 - (q16 * i2 * c2));
                    p16 = Math.Round(p16, 2);
                    arrws[0] = p16; // this was commented for excel and .net calculation difference on 25/april/2015  
                                    //arrws[0] =Math.Round((p16 - (1*-03)),2);  // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                                    //o16
                                    //SUM(Q16-(Q16*H2*C2))

                    o16 = (q16 - (q16 * h2 * c2));
                    o16 = Math.Round(o16, 2);
                    arrws[1] = o16;
                    //arrws[1] = Math.Round((o16 - (2 * .03)), 2); // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //n16
                    //=SUM(Q16-(Q16*G2*C2))

                    n16 = (q16 - (q16 * g2 * c2));
                    n16 = Math.Round(n16, 2);
                    arrws[2] = n16;
                    //arrws[2] = Math.Round((n16 - (3 * .03)), 2); // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //m16

                    //=SUM(Q16-(Q16*F2*C2))

                    m16 = (q16 - (q16 * f2 * c2));
                    m16 = Math.Round(m16, 2);
                    arrws[3] = m16;
                    //arrws[3] = Math.Round((m16 - (4 * .03)), 2); // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation


                    //k16
                    //=SUM(Q16-(Q16*E2*C2))

                    k16 = (q16 - (q16 * e2 * c2));
                    k16 = Math.Round(k16, 2);
                    arrws[4] = k16;
                    //arrws[4] = Math.Round((k16 - (6 * .03)), 2); // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation


                    //i16
                    //=SUM(Q16-(Q16*D2*C2))

                    i16 = (q16 - (q16 * d2 * c2));
                    i16 = Math.Round(i16, 2);
                    arrws[5] = i16;
                    //arrws[5] = Math.Round((i16 - (8 * .03)), 2); // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation

                    //g16
                    //suM(Q16-(Q16*C2*G15))
                    //tempws10 = g15 in excel
                    g16 = (q16 - (q16 * tempws10 * c2));
                    g16 = Math.Round(g16, 2);
                    arrws[6] = g16;
                    //arrws[6] = Math.Round((g16 - (10 * .03)), 2);  // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation


                    //e16
                    //=SUM(Q16-(Q16*E15*C2))
                    //tempws12 = e15 in excel
                    e16 = (q16 - (q16 * tempws12 * c2));
                    e16 = Math.Round(e16, 2);
                    arrws[7] = e16;
                    //arrws[7] = Math.Round((e16 - (12 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation


                    //c16
                    //=SUM(Q16-(Q16*C15*C2))
                    //tempws14 = c15 in excel
                    c16 = (q16 - (q16 * tempws14 * c2));
                    c16 = Math.Round(c16, 2);
                    arrws[8] = c16;
                    //arrws[8] = Math.Round((c16 - (14 * .03)), 2);  // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation

                    //a16
                    //=SUM(O16-(O16*A15*C2))
                    //tempws16 = a15 in excel
                    a16 = (o16 - (o16 * tempws16 * c2));
                    a16 = Math.Round(a16, 2);
                    arrws[9] = a16;
                    //arrws[9] = Math.Round((a16 - (16 * .03)), 2);  // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    #endregion

                    #region 20row

                    //row 20 calculation

                    //b20

                    //=SUM(C16-A16)

                    b20 = (c16 - a16);
                    b20 = Math.Round(b20, 2);
                    //d20
                    //=SUM(E16-C16)

                    d20 = (e16 - c16);
                    d20 = Math.Round(d20, 2);
                    //f20
                    //=SUM(G16-E16)

                    f20 = (g16 - e16);
                    f20 = Math.Round(f20, 2);

                    //h20
                    //=SUM(I16-G16)

                    h20 = (i16 - g16);
                    h20 = Math.Round(h20, 2);
                    //j20
                    //=SUM(K16-I16)

                    j20 = (k16 - i16);
                    j20 = Math.Round(j20, 2);
                    //k20
                    //=SUM(L19-J19)

                    k20 = (l19 - j19);
                    k20 = Math.Round(k20, 2);
                    //l20
                    //=SUM(M16-K16)

                    l20 = (m16 - k16);
                    l20 = Math.Round(l20, 2);
                    //m20
                    //=SUM(N19-L19)

                    m20 = (n19 - l19);
                    m20 = Math.Round(m20, 2);
                    //on20
                    //=SUM(P19-N19)

                    o20 = (p19 - n19);
                    o20 = Math.Round(o20, 2);
                    //q20
                    //=SUM(R19-P19)

                    q20 = (r19 - p19);
                    q20 = Math.Round(q20, 2);
                    //s20
                    //=SUM(T19-R19)

                    s20 = (t19 - r19);
                    s20 = Math.Round(s20, 2);
                    #endregion
                    #region 21row
                    //row 21 calculation

                    //b21
                    //=B20/1.618

                    b21 = (b20 / 1.618);
                    b21 = Math.Round(b21, 2);

                    //d21
                    //=D20/1.618

                    d21 = (d20 / 1.618);
                    d21 = Math.Round(d21, 2);

                    //f21
                    //=F20/1.618

                    f21 = (f20 / 1.618);
                    f21 = Math.Round(f21, 2);

                    //h21
                    //=H20/1.618

                    h21 = (h20 / 1.618);
                    h21 = Math.Round(h21, 2);
                    //j21
                    //=J20/1.618

                    j21 = (j20 / 1.618);
                    j21 = Math.Round(j21, 2);
                    //k21
                    //=K20/1.618

                    k21 = (k20 / 1.618);
                    k21 = Math.Round(k21, 2);
                    //l21
                    //=L20/1.618

                    l21 = (l20 / 1.618);
                    l21 = Math.Round(l21, 2);
                    //m21
                    //=M20/1.618

                    m21 = (m20 / 1.618);
                    m21 = Math.Round(m21, 2);
                    //o21
                    //=O20/1.618

                    o21 = (o20 / 1.618);
                    o21 = Math.Round(o21, 2);

                    //q21
                    //=Q20/1.618

                    q21 = (q20 / 1.618);
                    q21 = Math.Round(q21, 2);
                    //s21
                    //=S20/1.618

                    s21 = (s20 / 1.618);
                    s21 = Math.Round(s21, 2);
                    #endregion
                    #region 19row pending
                    // pending calculation for k19,m19,o19,q19,s19
                    //k19
                    //=SUM(J19+K21)

                    k19 = (j19 + k21);
                    k19 = Math.Round(k19, 2);
                    arrwr[9] = k19;
                    //m19
                    //=SUM(L19+M21)

                    m19 = (l19 + m21);
                    m19 = Math.Round(m19, 2);
                    arrwr[10] = m19;
                    //o19
                    //=SUM(N19+O21)

                    o19 = (n19 + o21);
                    o19 = Math.Round(o19, 2);
                    arrwr[11] = o19;
                    //q19
                    //=SUM(P19+Q21)
                    p21 = 0;
                    //q19 = (p19 + p21); // need to confirm with  commented here on 24/april/2015 for the value changed to q21
                    q19 = (p19 + q21);
                    q19 = Math.Round(q19, 2); // need to confirm with 
                    arrwr[12] = q19;
                    //s19

                    //=SUM(R19+S21)

                    s19 = (r19 + s21);
                    s19 = Math.Round(s19, 2);
                    arrwr[13] = s19;
                    #endregion
                    // pending calculation for k19 ends here
                    #region 16row pending
                    //l16

                    //=SUM(M16-L21)

                    l16 = (m16 - l21);
                    l16 = Math.Round(l16, 2);
                    arrws[10] = l16;
                    //arrws[10] = Math.Round((l16 - (5 * .03)), 2);  // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //j16
                    //=SUM(K16-J21)

                    j16 = (k16 - j21);
                    j16 = Math.Round(j16, 2);
                    arrws[11] = j16;
                    //arrws[11] = Math.Round((j16 - (7 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //h16
                    //=SUM(I16-H21)

                    h16 = (i16 - h21);
                    h16 = Math.Round(h16, 2);
                    arrws[12] = h16;
                    //arrws[12] = Math.Round((h16 - (9 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //d16
                    //=SUM(E16-D21)

                    d16 = (e16 - d21);
                    d16 = Math.Round(d16, 2);
                    arrws[13] = d16;
                    //arrws[13] = Math.Round((d16 - (13 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //f16
                    //=G16-F21

                    f16 = (g16 - f21);
                    f16 = Math.Round(f16, 2);
                    arrws[14] = f16;
                    //arrws[14] = Math.Round((f16 - (11 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //b16
                    //=SUM(C16-B21)

                    b16 = (c16 - b21);  // need to do row 21 calcultion prior to row 16

                    b16 = Math.Round(b16, 2);  // need to do row 21 calcultion prior to row 16#endregion
                    arrws[15] = b16;
                    //arrws[15] = Math.Round((b16 - (15 * .03)), 2);   // this factor was added by sunil shah on 25/april/2016 for excel and .net calculation
                    //from here The calculation will be mixed in nature
                    #endregion
                    #region 8row
                    e8 = "BUY";
                    f8 = "SELL";
                    //=IF(D8<D11,E8,F8)

                    g8 = (d8 < d11 ? e8 : f8);  //Wave1

                    h8 = "Wave1";

                    i8 = "BNS";



                    //n8
                    n8 = "BNR";

                    //p8
                    p8 = "wave2";

                    //q8

                    //=IF(T11>T8,E8,F8)

                    q8 = (t11 > t8 ? e8 : f8);   // wave 2

                    #endregion
                    #region 9row
                    //c9
                    //=IF(D10>D11,D11,D10)

                    c9 = (d10 > d11 ? d11 : d10);
                    c9 = Math.Round(c9, 2);
                    //d9
                    //=IF(D11<D10,D11,D10)

                    d9 = (d11 < d10 ? d11 : d10);
                    d9 = Math.Round(d9, 2);

                    //e9
                    e9 = "y";
                    f9 = "n";

                    //e12
                    //=IF(D8<D11,D8,D11)
                    e12 = (d8 < d11 ? d8 : d11);
                    e12 = Math.Round(e12, 2);

                    //=IF(G8=Q8,D11,E12)
                    d12 = (g8 == q8 ? d11 : e12);
                    d12 = Math.Round(d12, 2);

                    //g9
                    //=IF(D10>D12,E8,F8)

                    g9 = (d10 > d12 ? e8 : f8);   // wave 3

                    //h9
                    h9 = "wave3";
                    i9 = "WS";
                    // temperory disable then need to enable after database calculation
                    //k9 = 3107;  //ws

                    //l9
                    l9 = "CT";
                    // ct = ctld if  =IF(E18>R15,E19,R16)
                    // ct = cthd if  =IF(E18>R15,E19,R16)
                    // temperory disable then need to enable after database calculation
                    // m9 = 3235;  // wr

                    //n9
                    n9 = "WR";

                    //p9

                    p9 = "Wave4";


                    // moved from here on 23/april/2015
                    //t12
                    //IF(G8=Q8,T11,S12)
                    //// t12 = 0;
                    //// t12 = (g8 == q8 ? t11 : s12);  // need to confirm =IF(G8=Q8,T11,S12)
                    //// t12 = Math.Round(t12,2);
                    //q9

                    //=IF(T10>T12,E8,F8)

                    //// q9 = (t10 > t12 ? e8 : f8); // wave 4

                    //s9 calculation is pending 
                    //t9
                    //IF(T10>T11,T10,T11)


                    // move ends here for 23/april/2015

                    t9 = (t10 > t11 ? t10 : t11);
                    t9 = Math.Round(t9, 2);

                    //u9

                    //=IF(T11>T10,T11,T10)

                    u9 = (t11 > t10 ? t11 : t10);
                    u9 = Math.Round(u9, 2);


                    #endregion

                    // shifted below on 23/april/2015
                    // pending calculation for s8
                    //=IF(Q9=F8,E9,F9)
                    //// if (q9 == f8)
                    ////    s8 = e9;
                    ////  else
                    /////      s8 = f9;
                    // shifting ends here for 23/april/2015


                    // pending calculation for s8 end here
                    #region 10row



                    // move below on 11april2015

                    // moved below on 11april2015






                    ////  s10 = (q9 == g9 ? e9 : f9); shifted to below on 23/april/2015

                    #endregion
                    #region  11row

                    //k11
                    //=IF(D10>I6,E8,F8)
                    k11 = (d10 > i6 ? e8 : f8);
                    //m11
                    //=IF(T10<P6,F8,E8)

                    m11 = (t10 < p6 ? f8 : e8);



                    //s11
                    //=IF(S8=S10,E9,F9)

                    ////  s11 = (s8 == s10 ? e9 : f9);  shifted below on 23/april/2015
                    #endregion


                    // start from row 12 here on

                    #region 12row
                    //e12
                    //=IF(D8<D11,D8,D11)
                    e12 = (d8 < d11 ? d8 : d11);
                    e12 = Math.Round(e12, 2);
                    //d12

                    //=IF(G8=Q8,D11,E12)

                    d12 = (g8 == q8 ? d11 : e12);
                    d12 = Math.Round(d12, 2);
                    //n12
                    //=IF(M11=E8,(T10-P6)*0.618)

                    if (m11 == e8)
                    {
                        n12 = ((t10 - p6) * 0.618);
                        n12 = Math.Round(n12, 2);
                    }

                    else
                        n12 = 0;  // need to confirm

                    // moved to below on 11april2015 -- part 2
                    // moved to below on 11/april/2015 --- part 2 ends here
                    //j12
                    //=IF(K11=F8,(I6-D10)*0.618)

                    if (k11 == f8)
                    {

                        j12 = ((i6 - d10) * 0.618);
                        j12 = Math.Round(j12, 2);
                    }
                    else
                    {
                        j12 = 0; // because when condition is not matching then in excel it is written as false 
                                 // so we considered 0 as false here -- please mark as important
                    }



                    // moved to below from 11april2015 -- part 3

                    // moved ends here for 11april2015 --- part 3
                    //s12
                    //=IF(T8>T11,T8,T11)

                    s12 = (t8 > t11 ? t8 : t11);
                    s12 = Math.Round(s12, 2);

                    //t12
                    //IF(G8=Q8,T11,S12)

                    // t12 = (g8 == q8 ? t11 : t12); changed here on 23/april/2015 
                    t12 = (g8 == q8 ? t11 : s12);
                    t12 = Math.Round(t12, 2);

                    // shifted from above on 23/april/2015

                    //=IF(T10>T12,E8,F8)

                    q9 = (t10 > t12 ? e8 : f8); // wave 4

                    //s9 calculation is pending 
                    //t9
                    //IF(T10>T11,T10,T11)




                    // pending calculation for s8
                    //=IF(Q9=F8,E9,F9)
                    if (q9 == f8)
                        s8 = e9;
                    else
                        s8 = f9;

                    s10 = (q9 == g9 ? e9 : f9);
                    s11 = (s8 == s10 ? e9 : f9);
                    // shifting ends ehre from above of 23/april/2015


                    #endregion
                    #region 7row

                    // row 7 calculation



                    //b7
                    //IF(G9=E8,E9,F9)
                    b7 = (g9 == e8 ? e9 : f9);



                    //d7
                    //IF(G9=Q9,E9,F9)
                    d7 = (g9 == q9 ? e9 : f9);

                    //=IF(B7=D7,E9,F9)
                    //a7
                    a7 = (b7 == d7 ? e9 : f9);





                    //f7
                    //=IF(G8=E8,E9,F9)
                    f7 = (g8 == e8 ? e9 : f9);


                    //g7
                    //=IF(G8=Q8,E9,F9)

                    g7 = (g8 == q8 ? e9 : f9);

                    //e7
                    //(F7=G7,E9,F9)

                    e7 = (f7 == g7 ? e9 : f9);

                    //c7
                    //(A7=E7,A7,E7)
                    c7 = (a7 == e7 ? a7 : e7);



                    //h7
                    //(G7=F7,D12,E12)

                    h7 = Math.Round((g7 == f7 ? d12 : e12), 2);

                    //i7

                    //(G7=F7,D12,E12)
                    //=IF(I6>H7,I6,H7)

                    i7 = Math.Round((i6 > h7 ? i6 : h7), 2);

                    //k7
                    //=IF(D10<I6,D10,I6)

                    k7 = Math.Round((d10 < i6 ? d10 : i6), 2);

                    //m7
                    //=IF(P6>T10,P6,T10)

                    m7 = Math.Round((p6 > t10 ? p6 : t10), 2);




                    //r7
                    //=IF(Q8=G8,E9,F9)

                    r7 = (q8 == g8 ? e9 : f9);
                    //q7
                    //=IF(G8=F8,E9,F9)

                    q7 = (g8 == f8 ? e9 : f9);

                    //p7
                    //=IF(R7=Q7,T12,S12)

                    p7 = (r7 == q7 ? t12 : s12);

                    //n7
                    //=IF(P6<P7,P6,P7)

                    n7 = Math.Round((p6 < p7 ? p6 : p7), 2);


                    //s7
                    //=IF(Q7=R7,E9,F9)

                    s7 = (q7 == r7 ? e9 : f9);


                    #endregion
                    // here the calculation needs to be depend on wave1 to wave 4 
                    //wave 1 = g8
                    // wave 2 = q8

                    // wave 3 = g9
                    //wave 4 = q9
                    string trandview = GetFinalBuyorSell(g8, q8, g9, q9);
                    //txt_K13 = txt_K13 + " =calculated " + trandview;
                    k13 = trandview;
                    multiGainer.txt_K13 = trandview;
                    // added this on 12/may/2015 for not displaying trandeview in case of special case
                    if (!specialcase)
                    {
                        multiGainer.txt_K13_not = trandview;
                    }
                    else
                    {
                        multiGainer.txt_K13_not = "";
                    }
                    // addition of 12/may/2015 ends here
                    //wave1 and wave 2 is not same  and wave3 and wave 4 are same if wave 3 and wave 4 is buy then take k6 as bns and r4 as bnr
                    if ((g8 != q8) && (g9 == "BUY" && q9 == "BUY"))
                    {
                        k8 = k6;  // bns = k6
                                  //m8 = r6;  // bnr = r4 here it is r6
                                  //m8 = q6;  // changed here on 7/may/2015
                                  // added this on 15/may/2015 for making the changes of and BNR is R4 or idh whichever is high
                                  // commented here on 18/may/2015 to apply logic as follows
                                  // as per discussion with sunilbhai on 16/may/2013 the excel is in C:\Users\sunil.shah\Desktop\bhagirath_csv\needs_to_be_done_22feb2015\part8_12may2015
                                  // m8 = (q6 > t10 ? q6 : t10); 
                                  //sloution of if wave 3 and wave 4 is buy																				
                                  //step 1	bnr is r4 of idl which ever is high																			
                                  //	if avg <  r3(p6)		bns = s1 and brs is r4 or idh  which ever is higher
                                  //if avg > r3 then bns = s1 bnr is r5 or idh which ever is higer																				
                        if (m3 < p6)
                        {
                            //bnr is r4 or idh  which ever is higher
                            m8 = (q6 > t10 ? q6 : t10);
                        }
                        else
                        {
                            //bnr is r5 or idh which ever is higer
                            m8 = (r6 > t10 ? r6 : t10);
                        }
                        // addition of 18/may/2015 ends here
                    }
                    // added here on 8/may/2015
                    else if (g8 == "BUY" && g9 == "SELL" && q8 == "BUY" && q9 == "SELL")
                    {
                        k8 = h6; //bns = h6
                        m8 = n6;  // bnr = n6
                    }
                    // additio ends here for 8/may/2015         

                    //wave 1 and wave 2 are same and wave3 and wave 4 are same but oppose then wave1 and wave 2 then  then take k6 as bns and r4 as bnr
                    else if ((g8 == q8) && ((g9 == q9) && (g9 != g8 && q9 != q8)))
                    {
                        k8 = 0;
                        m8 = 0;

                        if (g8 == "SELL")
                        {
                            k8 = k6;

                            if (m3 < p6)
                            {
                                m8 = q6;
                            }
                            else
                            {
                                m8 = (r6 > t10 ? r6 : t10);
                            }
                        }
                        if (g8 == "BUY")
                        {
                            if (m3 > i6)
                            {
                                k8 = h6;
                            }
                            else
                            {
                                k8 = (g6 < d10 ? g6 : d10);
                            }
                            m8 = n6;
                        }
                        //   k8 = k6;  // bns = k6
                        //   //m8 = r6;  // bnr = r4 here it is r6

                        //    m8 = q6;  // changed here on 7/may/2015


                    }


                    // added new intrim condition on 22/april/2015 after discussion and email to sunilbhai
                    // wave 1 = buy ,wave 2 = sell,wave 3 = sell,wave 4 = sell
                    else if (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "SELL")
                    {
                        // commented here on 11/may/2015 as s4 is h6 and not s6
                        // k8 = (s4 < d10 ? s4 : d10);  // bns s4 or idl which ever is lower 
                        k8 = (h6 < d10 ? h6 : d10);  //bns
                        m8 = n6; // bnr= n6     
                                 // as per discussion with sunilbhai on 16/may/2013 the excel is in C:\Users\sunil.shah\Desktop\bhagirath_csv\needs_to_be_done_22feb2015\part8_12may2015
                                 //step 1)	avg > s3 then s4 or idl which ever is low																			
                                 //avg < s3	then now bns is s5 or idl which ever is lows
                        if (m3 > i6)
                        {
                            //avg > s3 then s4 or idl which ever is low	
                            k8 = (h6 < d10 ? h6 : d10);
                        }
                        else
                        {
                            ///avg < s3	then now bns is s5 or idl which ever is lows
                            k8 = (g6 < d10 ? g6 : d10);
                        }

                    }
                    // added one more condtion here on 21/may/2015 for bns calculation
                    else if (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "BUY")
                    {
                        k8 = (h6 < d10 ? h6 : d10);  //bns
                        if (m3 < i6)
                        {
                            k8 = (g6 < d10 ? g6 : d10);
                            k8 = Math.Round(k8, 2); //bns
                        }
                        // bnr is r3 so p6 here
                        m8 = p6;
                        m8 = Math.Round(m8, 2); //bnr
                    }
                    // addition of 21/may/2015 ends here
                    else
                    {
                        // pending calculation of k8 on 11/april/2015 = BNS
                        //=IF(C7=E9,I7,K7)
                        k8 = (c7 == e9 ? i7 : k7);
                        k8 = Math.Round(k8, 2); //bns



                        // pending calculation of s9
                        // =IF(S7=S11,S7,S11)
                        if (s7 == s11)
                            s9 = s7;
                        else
                            s9 = s11;
                        // pendign calculation of s9 ends here
                        // copied from row no 8
                        //m8

                        //=IF(S9=E9,N7,M7)

                        m8 = (s9 == e9 ? n7 : m7);  // in new excel sheet formula is not given 
                        m8 = Math.Round(m8, 2);   //BNR

                    }
                    // moved out of if loop on 24/april/2015 as in some of the cases it is not printing the value
                    multiGainer.txt_k8 = Convert.ToString(k8);
                    multiGainer.txt_m8 = Convert.ToString(m8);
                    // move from above on 11april2015



                    //=IF(E18>R15,E19,R16)
                    k9 = 0;
                    m9 = 0;
                    l10 = (e18 > r15 ? e19 : r16);  // need to confirm for CT
                                                    // calculation of WS and WR onbasis of ct 
                                                    // for CTHD search on tempws value == r16 here
                                                    // for CTLD search on tempwr valie == e19 here
                    Array.Sort(arrwr);  //sorting of array for calculation
                    Array.Sort(arrws);  // sorting of array for calculation
                                        //arrws
                    if (l10 == r16)  // if ct = CTHD
                    {
                        // ws level should be below bns level from tempws less then CTHD
                        // bns  = k8
                        object myObject = k8;  // k8 = bns
                        int myIndex = Array.BinarySearch(arrws, myObject);
                        if (myIndex < 0)
                        {
                            myIndex = Math.Abs(myIndex) - 1;
                        }
                        multiGainer.txt_K9 = arrws[(myIndex - 1)].ToString();  // calculated ws
                                                                               //txt_K9 = arrws[(myIndex)].ToString();  // calculated ws  changed on 15april2015
                        k9 = arrws[(myIndex - 1)];
                        // wr level  should be above bnr level from tempws above cthd
                        // bnr = m8
                        myObject = m8; //bnr
                        myIndex = Array.BinarySearch(arrws, myObject);
                        if (myIndex < 0)
                        {
                            myIndex = Math.Abs(myIndex) - 1;
                        }
                        //txt_M9 = arrws[(myIndex-1)].ToString(); // calculted wr
                        // if nearest value of bnr is not availabel with in arrws then take cthd value as wr on 1/may/2015
                        int arrlength = arrws.Length;
                        if (myIndex == arrlength)
                        {
                            multiGainer.txt_M9 = q16.ToString(); // cthd as on 1/may/2015
                            m9 = q16;
                        }
                        else
                        {
                            multiGainer.txt_M9 = arrws[(myIndex)].ToString();  // changed here on 15/april/2015
                            m9 = arrws[myIndex];
                        }
                        //m9 = arrws[myIndex-1];
                        //m9 = arrws[myIndex];
                    }

                    if (l10 == e19) // if ct = CTLD
                    {

                        // ws level should be below bns level from tempws less then CTHD
                        // bns  = k8
                        object myObject = k8;  // k8 = bns
                        int myIndex = Array.BinarySearch(arrwr, myObject);
                        if (myIndex < 0)
                        {
                            myIndex = Math.Abs(myIndex) - 1;
                        }
                        multiGainer.txt_K9 = arrwr[(myIndex - 1)].ToString();  // calculated ws
                        k9 = arrwr[(myIndex - 1)];
                        // wr level  should be above bnr level from tempws above cthd
                        // bnr = m8
                        myObject = m8; //bnr
                        myIndex = Array.BinarySearch(arrwr, myObject);
                        if (myIndex < 0)
                        {
                            myIndex = Math.Abs(myIndex) - 1;
                        }
                        if (myIndex == arrwr.Count())
                        {
                            myIndex = myIndex - 1;
                        }

                        multiGainer.txt_M9 = arrwr[myIndex].ToString(); // calculted wr
                        m9 = arrwr[myIndex];
                        // commented temperoray for testing on 4/july/2015
                        //multiGainer.txt_M9 = arrwr[myIndex - 1].ToString(); // calculted wr
                        //m9 = arrwr[myIndex - 1];



                    }

                    //i10
                    //=SUM(K8-K9)
                    i10 = (k8 - k9);
                    i10 = Math.Round(i10, 2);
                    //h10
                    //=SUM(I10/1.618)

                    h10 = (i10 / 1.618);
                    h10 = Math.Round(h10, 2);
                    // k10 
                    //=SUM(K9+H10)

                    k10 = (k9 + h10);
                    k10 = Math.Round(k10, 2);
                    //l10

                    //o10
                    //=SUM(M9-M8)
                    o10 = (m9 - m8);
                    o10 = Math.Round(o10, 2);
                    //p10
                    //=SUM(O10/1.618)
                    // according to new it is o10/1.382
                    p10 = (o10 / 1.618);
                    // p10 = (o10 / 1.382); COMMENTED HERE ON 10MARCH2015
                    p10 = Math.Round(p10, 2);

                    //m10
                    //=SUM(M9-P10)  old formula
                    //=SUM(M8+P10)  new formula
                    //m10 = (m8 + p10); COMMENTED ON 10MARCH2015
                    //=SUM(M9-P10)  old formula
                    m10 = (m9 - p10);
                    m10 = Math.Round(m10, 2);

                    //s10
                    //=IF(Q9=G9,E9,F9)
                    //  part 2 of moving from top 
                    // before going in special case we need to have a rough value of i12 and o12 
                    // so we are calculating as normal casee of i12 and o12 on 12/may/2015
                    // need to confirm with suniljoshi for this 
                    i12 = (m11 == e8 ? k10 + n12 : k10);
                    o12 = (k11 == f8 ? (m10 - j12) : m10);
                    // moving the logic of special case on 12/may/2015 at the top most priority as per discussion and email from suniljoshi on 11/may/2015
                    // special case as per discussion with sunilbhai on 7/may/2015

                    // special case no clear direction 1 wave1 = buy, wave3 = buy, wave2 = sell wave4 = sell the same case applied in calculation of g13 also check there 
                    // added one more condtion of specialsheet14 and specialsheet 17 at the end on 10/may/2015
                    if ((g8 == "BUY" && g9 == "BUY" && q8 == "SELL" && q9 == "SELL") || (g8 == "SELL" && g9 == "BUY" && q8 == "BUY" && q9 == "SELL") || (g8 == "SELL" && g9 == "SELL" && q8 == "BUY" && q9 == "BUY") || (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "BUY"))
                    {
                        specialcase = true;
                        showtrandview = false;
                        // added this on 21/may/2015 for removing the adjustment in special case
                        i12 = (m11 == e8 ? k10 : k10);
                        o12 = (k11 == f8 ? m10 : m10);
                        // addition of special case ends here of 21/may/2015
                        //=IF(P7>T10,P7,T10)
                        q12 = (p7 > t10 ? p7 : t10);
                        //=IF(H7<D10,H7,D10)
                        g12 = (h7 < d10 ? h7 : d10);
                        // =SUM(Q12*0.00013)+Q12
                        r11 = (q12 + (q12 * 0.00013));
                        r11 = Math.Round(r11, 2);
                        //(g12-(g12*0.00013)
                        f11 = (g12 - (g12 * 0.00013));
                        f11 = Math.Round(f11, 2);
                        if (i12 < f11) // buying sl less then f11 then buying sl is f11
                        {
                            i12 = f11;
                        }
                        // added for specialsheet14 on 10/may/2015
                        if ((g8 == "SELL" && g9 == "BUY" && q8 == "SELL" && q9 == "BUY"))
                        {
                            //=SUM(Q12-G12)
                            c22 = (q12 - g12);
                            c22 = Math.Round(c22, 2);
                            //=SUM(C22*0.786)
                            d22 = (c22 * 0.786);
                            d22 = Math.Round(d22, 2);
                            //=SUM(Q12-D22)
                            d24 = (q12 - d22);
                            d24 = Math.Round(d24, 2);
                            //HERE WE FOUND BUY SL 17975.02 THAT IS BELOW S3 SO WE TAKE BUY SL 17975.02(D24)
                            if (d24 < i6)
                            {
                                i12 = d24;  // buying sl 
                            }
                        }
                        // addition for specialsheet14 of 10/may/2015 ends here 
                        if (o12 > r11) // if selling sl is greater than r11 then selling sl is r11
                        {
                            o12 = r11;
                        }
                        // commented ohere on 22/may/2015
                        // added for specialsheet17 on 10/may/2015 
                        //     if ((g8 == "BUY" && g9 == "SELL" && q8 == "BUY" && q9 == "SELL"))
                        //     {
                        //         //=SUM(Q12-G12)
                        //         c22 = (q12 - g12);
                        //         c22 = Math.Round(c22, 2);
                        //         //=SUM(C22*0.786)
                        //         d22 = (c22 * 0.786);
                        //         d22 = Math.Round(d22, 2);
                        ///        //=SUM(G12+D22)
                        //         d24 = (g12 + d22);
                        //         d24 = Math.Round(d24, 2);
                        //         //HERE WE FOUND SELL SL 3962.89 (D24) ABOVE R3 SO WE TAKE SELL SL 3962.89 (D24)
                        //         if (d24 > p6)
                        //         {
                        //             o12 = d24;  // selling sl
                        //         }
                        //   }
                        // comment ends here 
                        // addition ends here for specialsheet17 on 10/may/2015

                    }

                    if (g8 == "BUY" && g9 == "SELL" && q8 == "BUY" && q9 == "SELL")
                    {
                        specialcase = true;
                        showtrandview = true;
                        i12 = (m11 == e8 ? k10 : k10);
                        o12 = (k11 == f8 ? m10 : m10);
                        if (o12 < p6)
                            o12 = p6;
                        if (i12 > h6)
                            i12 = s6;
                        //    g13 = ((o12 - i12) * 0.786);
                        //    h13 = ((o12 - i12) * 0.786);
                    }

                    if ((g8 == "SELL" && g9 == "BUY" && q8 == "SELL" && q9 == "BUY"))
                    {
                        specialcase = true;
                        showtrandview = true;
                        i12 = (m11 == e8 ? k10 : k10);
                        o12 = (k11 == f8 ? m10 : m10);
                        if (o12 < q6)
                            o12 = q6;
                        if (i12 > i6)
                            i12 = i6;
                    }
                    // special case ends here







                    // logic of special case ends here on 12/may/2015

                    // need to make changes as per new logic given by sunil on 11april2015
                    //wave 1 = g8
                    // wave 3 = g9
                    // wave 2 = q8
                    //wave 3 = q9
                    //wave1 and wave 2 is not same  and wave3 and wave 4 are same if wave 3 and wave 4 is buy then take k6 as bns and r4 as bnr
                    if (((g8 != q8) && (g9 == "BUY" && q9 == "BUY")) && !specialcase)
                    {
                        showtrandview = true;
                        i12 = (m11 == e8 ? k10 : k10);
                        // new adjustment as per discussion with sunilbhai on 18/april/2015
                        i12 = (i12 - (i12 * 0.00013));
                        i12 = Math.Round(i12, 2);
                        o12 = (k11 == f8 ? (m10) : m10);
                        // new adjustment as per discussion with sunilbhai on 18/april/2015
                        o12 = (o12 + (o12 * 0.00013));
                        o12 = Math.Round(o12, 2);

                    }
                    //wave 1 and wave 2 are same and wave3 and wave 4 are same but oppose then wave1 and wave 2 then  then take k6 as bns and r4 as bnr
                    else if (((g8 == q8) && ((g9 == q9) && (g9 != g8 && q9 != q8))) && !specialcase)
                    {
                        showtrandview = true;
                        i12 = (m11 == e8 ? k10 : k10);
                        // new adjustment as per discussion with sunilbhai on 18/april/2015
                        i12 = (i12 - (i12 * 0.00013));
                        i12 = Math.Round(i12, 2);
                        o12 = (k11 == f8 ? (m10) : m10);
                        // new adjustment as per discussion with sunilbhai on 18/april/2015
                        o12 = (o12 + (o12 * 0.00013));
                        o12 = Math.Round(o12, 2);

                    }

                    else if (!specialcase)
                    {

                        // changes ends here
                        //i12
                        //=IF(M11=E8,K10+N12,K10)
                        // according to new excel it is IF(M11=E8,K10,K10)
                        showtrandview = true;
                        i12 = (m11 == e8 ? k10 + n12 : k10);
                        // new adjustment as per discussion with sunilbhai on 18/april/2015
                        // intreim condtion added on 24/april/2015 of wave1 = buy wave2 = sell wave3= sell wave4 = sell then do not adjust j12 in o12 as per problem sheet worksheet 1
                        if (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "SELL")
                        {
                        }
                        else
                        {
                            i12 = (i12 - (i12 * 0.00013));
                        }
                        //i12 = (i12 - (i12 * 0.00013)); // commented here on 24/april/2015 for intrim condtion adjustment 
                        i12 = Math.Round(i12, 2);
                        // part 3 of moving from top


                        // intreim condtion added on 24/april/2015 of wave1 = buy wave2 = sell wave3= sell wave4 = sell then do not adjust j12 in o12 as per problem sheet worksheet 1
                        if (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "SELL")
                        {
                            o12 = m10;
                        }
                        else
                        {
                            //012
                            //=IF(K11=F8,M10-J12,M10)
                            // accprdomg to new excel  =IF(K11=F8,M10,M10)
                            o12 = (k11 == f8 ? (m10 - j12) : m10);
                            o12 = (o12 + (o12 * 0.00013));  // moved inside as in some cases we do not required this to be done on 24/april/2015
                                                            // o12 = (k11 == f8 ? m10  : m10);  // excel sheet formula are differing 
                                                            // new adjustment as per discussion with sunilbhai on 18/april/2015
                        }


                        o12 = Math.Round(o12, 2);
                    }

                    //intrim condition (2-2)  --> wave 1: buy, wave 2 : buy, wave3 :sell,wave4 : sell added on 22/april/2015 
                    if ((g8 == "BUY" && q8 == "BUY" && g9 == "SELL" && q9 == "SELL") && !specialcase)
                    {
                        showtrandview = true;
                        if (d10 < i12)
                        {
                            if (l10 == "CTHD")
                            {
                                //take level from cthd below idl and that will be buying sl) tempws is cthd level 
                                object myObject = d10;  // d10 = idl
                                int myIndex = Array.BinarySearch(arrws, myObject);
                                if (myIndex < 0)
                                {
                                    myIndex = Math.Abs(myIndex) - 1;
                                }
                                i12 = arrwr[(myIndex - 1)];
                            }
                            if (l10 == "CTLD")
                            {
                                //take level from ctld below idl and that will be buying sl tempwr level is ctld level
                                // ws level should be below bns level from tempws less then CTHD
                                // bns  = k8
                                object myObject = d10;  // d10 = idl
                                int myIndex = Array.BinarySearch(arrwr, myObject);
                                if (myIndex < 0)
                                {
                                    myIndex = Math.Abs(myIndex) - 1;
                                }
                                i12 = arrwr[(myIndex - 1)];
                            }
                        }
                        o12 = n6; //selling sl would be R1= n6
                    }
                    //intrim condition 4-3 --> wave 1 = sell,wave 2 = sell,wave 3 = buy,wave 4 = buy
                    if ((g8 == "SELL" && q8 == "SELL" && g9 == "BUY" && q9 == "BUY") && !specialcase)
                    {
                        showtrandview = true;
                        //idh > selling sl
                        if (t10 > o12)
                        {
                            if (l10 == "CTHD")
                            {
                                // take level from cthd above idh and that will be selling sl
                                object myObject = t10; //bnr
                                int myIndex = Array.BinarySearch(arrws, myObject);
                                if (myIndex < 0)
                                {
                                    myIndex = Math.Abs(myIndex) - 1;
                                }
                                o12 = arrwr[myIndex];
                            }
                            if (l10 == "CTLD")
                            {
                                //take level from ctld above idh and that will be selling sl
                                object myObject = t10; //bnr
                                int myIndex = Array.BinarySearch(arrwr, myObject);
                                if (myIndex < 0)
                                {
                                    myIndex = Math.Abs(myIndex) - 1;
                                }
                                o12 = arrwr[myIndex];
                            }
                        }
                        i12 = k6; // need to confirm with sunilbhai buying sl would be s1 check it tomorrow using excel sheet
                    }
                    // moved from above on 11april2015 ends here
                    // if buysl should not be greater than i6 if it is greter than i6 then buysl is i6 as per sunilbhai email 
                    // Buy sl (i 12) must not be greater than S3(i6) else S3(i6) is a Buy sl  and Sell sl  (o12) must not be lower lower than R3 (p6) else R3 (p6) is Sell sl.
                    //// removed on 21/may/2015 for problem 
                    //            if (i12 > i6)
                    //            {
                    //                i12 = i6;
                    //            }
                    //// Sell sl  (o12) must not be lower lower than R3 (p6) else R3 (p6) is Sell sl.
                    //            if (o12 < p6)
                    //            {
                    //                o12 = p6;
                    //            }

                    //// ends here removed on 21/may/2015 for problem 
                    // moved logic of special case from here on 12/may/2015 as per top most priority as per discussion and email from suniljoshi on 11/may/2015
                    // moved logic for special case from here on 12/may/2015
                    //copy from row 8 ends here
                    // pending calculation of j10 , n10
                    //=SUM(L8-I12)
                    j10 = (l8 - i12);
                    j10 = Math.Round(j10, 2);
                    //=SUM(O12-L8)
                    n10 = (o12 - l8);
                    n10 = Math.Round(n10, 2);
                    // pending calculation of j10,n10 ends here
                    // pending calculation of j11 and n11
                    //j11
                    //=IF(J10>N10,F8,E8)

                    j11 = (j10 > n10 ? f8 : e8);  // band view
                    multiGainer.txt_H12 = j11;
                    //n11
                    //IF(N10>J10,E8,F8)

                    n11 = (n10 > j10 ? e8 : f8);
                    // pending calculation of j11 and n11 ends here

                    #region 13row
                    //=SUM(O12-I12)*0.576
                    // temperory disable then need to enable after database calculation
                    //k13 = "sell";

                    // special case no clear direction 1 wave1 = buy, wave3 = buy, wave2 = sell wave4 = sell the same case applied in calculation of g13 also check there 
                    if ((g8 == "BUY" && g9 == "BUY" && q8 == "SELL" && q9 == "SELL") || (g8 == "SELL" && g9 == "BUY" && q8 == "BUY" && q9 == "SELL") || (g8 == "SELL" && g9 == "SELL" && q8 == "BUY" && q9 == "BUY") || (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "BUY"))
                    {
                        specialcase = true;
                        showtrandview = false;
                        g13 = ((o12 - i12) * 0.75);
                    }
                    else
                    {
                        // on 22/may/2015
                        if ((g8 == "BUY" && g9 == "SELL" && q8 == "BUY" && q9 == "SELL") || (g8 == "SELL" && g9 == "BUY" && q8 == "SELL" && q9 == "BUY"))
                        {
                            showtrandview = true;
                            g13 = ((o12 - i12) * 0.786);
                        }
                        else
                        {

                            g13 = ((o12 - i12) * 0.576);
                        }
                    }
                    g13 = Math.Round(g13, 2);
                    //h13
                    //=SUM(O12-I12)*0.87
                    // special case intrim condition as per email h13 = sum(o12-i12)*79
                    //if  Buy sl (i12) must not be below TDL (d8) and Sell Sl (o12) must not be above TDH for Special cases only.
                    // done this on 2/may/2015
                    // special case no clear direction 1 wave1 = buy, wave3 = buy, wave2 = sell wave4 = sell the same case applied in calculation of g13 also check there 
                    if ((g8 == "BUY" && g9 == "BUY" && q8 == "SELL" && q9 == "SELL") || (g8 == "SELL" && g9 == "BUY" && q8 == "BUY" && q9 == "SELL") || (g8 == "SELL" && g9 == "SELL" && q8 == "BUY" && q9 == "BUY") || (g8 == "BUY" && g9 == "SELL" && q8 == "SELL" && q9 == "BUY"))
                    {
                        specialcase = true;
                        showtrandview = false;
                        h13 = ((o12 - i12) * 0.75);
                    }
                    else
                    {
                        // on 22/may/2015
                        if ((g8 == "BUY" && g9 == "SELL" && q8 == "BUY" && q9 == "SELL") || (g8 == "SELL" && g9 == "BUY" && q8 == "SELL" && q9 == "BUY"))
                        {
                            showtrandview = true;
                            h13 = ((o12 - i12) * 0.786);
                        }
                        else if (012 > d8 && i12 < o12)
                        {
                            showtrandview = true;
                            h13 = ((o12 - i12) * 0.79);
                        }
                        else
                        {
                            showtrandview = true;
                            h13 = ((o12 - i12) * 0.87);
                        }

                    }
                    h13 = Math.Round(h13, 2);
                    //j13
                    //=IF(K13=E8,O12-G13,O12-H13)

                    j13 = (k13 == e8 ? (o12 - g13) : (o12 - h13));   // s-bap
                    j13 = Math.Round(j13, 2);
                    //n13
                    //=IF(K13=E8,I12+H13,I12+G13)

                    n13 = (k13 == e8 ? (i12 + h13) : (i12 + g13));   // r-bap
                    n13 = Math.Round(n13, 2);

                    #endregion
                    #region fixed value display
                    multiGainer.txt_D6 = Convert.ToString(d6);
                    multiGainer.txt_E6 = Convert.ToString(e6);
                    multiGainer.txt_F6 = Convert.ToString(f6);
                    multiGainer.txt_G6 = Convert.ToString(g6);
                    multiGainer.txt_H6 = Convert.ToString(h6);
                    multiGainer.txt_I6 = Convert.ToString(i6);
                    multiGainer.txt_J6 = Convert.ToString(j6);
                    multiGainer.txt_K6 = Convert.ToString(k6);
                    multiGainer.txt_N6 = Convert.ToString(n6);
                    multiGainer.txt_O6 = Convert.ToString(o6);
                    multiGainer.txt_P6 = Convert.ToString(p6);
                    multiGainer.txt_Q6 = Convert.ToString(q6);
                    multiGainer.txt_R6 = Convert.ToString(r6);
                    multiGainer.txt_S6 = Convert.ToString(s6);
                    multiGainer.txt_T6 = Convert.ToString(t6);
                    multiGainer.txt_U6 = Convert.ToString(u6);
                    // txt_H12 ="";
                    //txt_K13 = k13; commented here on 24/april/2015 to display trandview from calculation
                    multiGainer.txt_J13 = Convert.ToString(j13);
                    multiGainer.txt_N13 = Convert.ToString(n13);
                    multiGainer.txt_M6 = Convert.ToString(m6);
                    // added this on 20/may/2015 for special case trandview display to be disabled
                    if (specialcase && !showtrandview)
                    {
                        multiGainer.txt_K13_not = "";
                    }
                    else
                    {
                        multiGainer.txt_K13_not = trandview;
                    }



                    #endregion
                    #region 23row

                    //row 23 calculation

                    //j23
                    //=SUM(O12-I12)

                    j23 = (o12 - i12); //range
                    j23 = Math.Round(j23, 2);
                    // TextBox3 = j23.ToString(); // this value is assigned in row25 calculation
                    #endregion

                    #region 24row
                    //row 24 calculation

                    i24 = "Positive";
                    j24 = "Negative";
                    #endregion

                    #region 25row
                    // row 25 calculation

                    //i25
                    //=SUM(L8-I12)*100/J23

                    i25 = ((l8 - i12) * 100 / j23);   // negative
                    i25 = Math.Round(i25, 2);

                    //j25
                    //=SUM(O12-L8)*100/J23

                    j25 = ((o12 - l8) * 100 / j23);  // positive
                    j25 = Math.Round(j25, 2);

                    //if (Math.Abs(i25) > 100 || Math.Abs(j25) > 100)
                    if (i25 < 0 || i25 >= 100 || j25 < 0 || j25 >= 100)  // after discussion with sunilbhai if any value is below 0 or greate than 100 will hit stoploss
                    {
                        // commented here if stoploss is hit i.e percentage is greater than 100% then nothing to be displayed as per suniljoshi on 7/may/2015
                        // TextBox1 = i25.ToString() + " ( STOPLOSS HIT )"; 
                        multiGainer.TextBox1 = "";  // negative value 
                        multiGainer.TextBox2 = "";  // positive value
                        multiGainer.TextBox3 = "";  // range value
                                                    // uint myUint = (uint)Math.Round(50.11, 0);
                                                    // need to check for setting width of label at run time by sunil shah
                                                    ////multiGainer.Label15.Width = new Unit((Math.Round(Math.Abs(50.01), 0)) * 2);
                                                    /////multiGainer.Label16.Width = new Unit((Math.Round(Math.Abs(50.01), 0)) * 2); ;
                    }
                    else
                    {
                        multiGainer.TextBox1 = i25.ToString();  // negative value
                        multiGainer.TextBox2 = j25.ToString(); // positive value
                        multiGainer.TextBox3 = j23.ToString();  // range value
                                                                //  uint myUint = (uint)Math.Round(i25, 0);
                                                                // need to check for setting width of label at run time by sunil shah
                                                                ////multiGainer.Label15.Width = new Unit((Math.Round(Math.Abs(i25), 0)) * 2);
                                                                /////multiGainer.Label16.Width = new Unit((Math.Round(Math.Abs(j25), 0)) * 2);

                    }

                    //            if (Math.Abs(i25) > 100)
                    //            {
                    //                // commented here if stoploss is hit i.e percentage is greater than 100% then nothing to be displayed as per suniljoshi on 7/may/2015
                    //               // TextBox1 = i25.ToString() + " ( STOPLOSS HIT )"; 
                    //                TextBox1 = "";  // negative value 
                    //                TextBox2 = "";  // positive value
                    //                TextBox3 = "";  // range value/
                    //
                    //            }
                    //            else
                    //            {
                    //                TextBox1 = i25.ToString();  // negative value
                    //                TextBox2 = j25.ToString(); // positive value
                    //                TextBox3 = j23.ToString();  // range value
                    //                uint myUint = (uint)Math.Round(i25, 0);
                    //                ///Label15.Width = myUint+"px";
                    //                Label15.Width = new Unit((Math.Round(Math.Abs(i25), 0)) * 2);
                    //                Label16.Width = new Unit((Math.Round(Math.Abs(j25), 0)) * 2);


                    //            }
                    // this is commented as we have added above loop for not considering if any of the percentage is greate than 100 on 8/may/2015 as per
                    // discussion with suniljoshi if any percentage is greater than 100% then we are not showing positve,negative and range on 7/may/2015 in office
                    ////// uint myUint = (uint)Math.Round(i25, 0);
                    //////Label15.Width = new Unit((Math.Round(Math.Abs(i25), 0))*2);
                    ///// if (Math.Abs(j25) > 100)
                    ////// {
                    // commented here if stoploss is hit i.e percentage is greater than 100% then nothing to be displayed as per suniljoshi on 7/may/2015
                    //TextBox2 = j25.ToString()+  " ( STOPLOSS HIT )";
                    //////    TextBox1 = "";  // negative value
                    //////    TextBox2 = "";  // positive value
                    //////    TextBox3 = "";  // range value 
                    ////// }
                    //////else
                    ///// {
                    /////   TextBox2 = j25.ToString();
                    //////  TextBox3 = j23.ToString();  // range value
                    /////// }
                    /////Label16.Width = new Unit((Math.Round(Math.Abs(j25), 0))*2);

                    // commented of 8/may/2015 ends here for not displaying anything if percentge is greate than 100

                    // added here on 5/may/2015 for interim condition problem
                    if (o12 > d8 && i12 < o12)
                    {
                    }
                    else
                    {
                        //=SUM(O12-I12)*0.75
                        h24 = (o12 - i12) * 0.75;
                        h24 = Math.Round(h24, 2);
                        //=SUM(O12-H24)
                        // added on 20/may/2014 for s-bap and r-bap problem
                        if (!goforrecaculation)
                        {
                            j13 = (o12 - h24);// s-bap
                            j13 = Math.Round(j13, 2);
                            //=SUM(I12+H24)
                            n13 = (i12 + h24); // r-bap;
                            n13 = Math.Round(n13, 2);
                            multiGainer.txt_J13 = Convert.ToString(j13);
                            multiGainer.txt_N13 = Convert.ToString(n13);
                        }

                    }
                    // addition ends here for 5/may/2015
                    multiGainer.txt_I12 = i12.ToString();  // buy sl
                    multiGainer.txt_O12 = o12.ToString();   // sell sl

                    // checking for reversal call 
                    //double possibilitespercent = 0;
                    // this needs to be done for the first time only and not during recalculation is done on 5/may/2015
                    // if special case is there then no need for checking for reverse call and band view call as on 9/may/2015 with suniljoshi in mroning
                    if (!goforrecaculation && !specialcase)
                    {
                        string[] reversalbuy = new string[3];
                        string[] reversalsale = new string[3];

                        //=IF(D10>K6,E8,F8)
                        reversalbuy[0] = (d10 > k6 ? e8 : f8);
                        if (reversalbuy[0] == "BUY")
                            buycount = buycount + 1;

                        // as per new excel sheet on 1/may/2015
                        //=IF(T10>N6,E8,F8)
                        //=IF(D10>D11,E8,F8)
                        reversalbuy[1] = (d10 > d11 ? e8 : f8);
                        if (reversalbuy[1] == "BUY")
                            buycount = buycount + 1;

                        //=IF(M3>M6,E8,F8)
                        reversalbuy[2] = (m3 > m6 ? e8 : f8);
                        if (reversalbuy[2] == "BUY")
                            buycount = buycount + 1;





                        //=IF(T10<N6,F8,E8)
                        reversalsale[0] = (t10 < n6 ? f8 : e8);
                        if (reversalsale[0] == "SELL")
                            salcount = salcount + 1;

                        //=IF(D10>K6,E8,F8)
                        //=IF(T10<T11,F8,E8) on 2/may/2015
                        reversalsale[1] = (t10 < t11 ? f8 : e8);
                        if (reversalsale[1] == "SELL")
                            salcount = salcount + 1;

                        //=IF(M3<M6,F8,E8)
                        reversalsale[2] = (m3 < m6 ? f8 : e8);
                        if (reversalsale[2] == "SELL")
                            salcount = salcount + 1;

                    }
                    // this is end for goforrecaclulation logic on 5/may/2015

                    //commented on 1/may/2015
                    //=IF(D10>D11,E8,F8)
                    //reversalbuy[1] = (d10 > d11 ? e8 : f8);
                    //if (reversalbuy[1] == "BUY")
                    //    buycount = buycount + 1;

                    //=IF(T10<T11,F8,E8)
                    // reversalsale[1] = (t10 < t11 ? f8 : e8);
                    // if (reversalsale[1] == "SELL")
                    //     salcount = salcount + 1;


                    // 1/may commented ends here

                    // if the possibilities is equal to 100% then only show revert call and do other calculation 
                    //possibilitespercent = ((100 * buycount) / 3);
                    if (((100 * buycount) / 3) >= 100)
                    {
                        // if trandview is sell then check for buycount for equal or more than 2 then buy is the result
                        if (k13 == "SELL" && buycount >= 2)
                        {

                            // id trendview is sell then decrease the value of average upto value of m6 and then recalculate the value again
                            // and in that value of k6 is taken as buyingsl and in that k6 value add 0.00013 of k6 to get the final buyingsl.
                            // as per whatsapp on 2/may/2015
                            // in revert call is average is below m6 or above m6  then make both m6 and average same
                            // by making changes in average value try to get the nearest value to m6 so final average value should be equal or nearer to m6
                            // changes done on 2/may/2016
                            // m3 is average
                            double tempm3 = m3;
                            double tempm6 = 0;
                            double testmv = 0;
                            if (m3 != m6 && !(goforrecaculation))
                            {

                                // changed here on 17/may/2015 for avgrage value problem of 13/may/2015
                                //for (double i = m3-100; i <= m3; i--)
                                // for (double i = m3; i <= m3-100; i--)
                                double i = Math.Round(m3, 0);
                                while (i > 0)
                                {
                                    tempm3 = i;
                                    tempm6 = (tempm3 + m4 + m5) / 3;
                                    tempm6 = Math.Round(tempm6, 2);
                                    if (tempm3 <= tempm6)
                                    {
                                        // tempm3 = tempm3 - 1;
                                        tempm6 = (tempm3 + m4 + m5) / 3;
                                        tempm6 = Math.Round(tempm6, 2);
                                        testmv = tempm6;
                                        m3 = tempm3;
                                        // goto outprint1;
                                        multiGainer.txt_m3 = m3.ToString(); // changed on 20/may/2015
                                        goforrecaculation = true;
                                        goto Recalculate; // label define at the top for recalculating 
                                    }
                                    i = i - 1;
                                }
                            }
                        outprint1:
                            // changed on 25/april/216 So if in such situation if we get input of IDL below buy sl of reverse call then reverse call must not reflect in output.
                            // d10 is idl and k6 is buying sl of reverse call 
                            //if (k6 > d10) // changed here on 1/may/2015 for changing k6 to i12 i.e buying sl
                            // if (i12 > d10) // changed here on 3 /may/22015 
                            if (d10 > k6)
                            {
                                multiGainer.txt_reversecall = "BUY";
                                double intrimk6value = 0;
                                intrimk6value = (0.00013 * k6);
                                k6 = k6 + intrimk6value;
                                k6 = Math.Round(k6, 2);
                                multiGainer.txt_Buysl = Convert.ToString(k6);
                                multiGainer.txt_buyt1 = Convert.ToString(o6);
                                multiGainer.txt_buyt2 = Convert.ToString(p6);
                                multiGainer.Txt_buyt3 = Convert.ToString(q6);
                                multiGainer.txt_reverseBAP = Convert.ToString(m3); // as per output excel it is average or m6 value
                                                                                   // multiGainer.txt_possibilities = Convert.ToString(((100 * buycount) / 3));
                                                                                   // here we need to change the value of s-bap and r-bap if reverse call is there and 
                                                                                   // if trandview is sell then take s-bap take 0.87 and r-bap is 0.576 as per whatapps on 19/may/2015
                                j13 = (j13 * 0.87);  // s-bap
                                j13 = Math.Round(j13, 2);
                                n13 = (n13 * 0.576);  //r -bap
                                n13 = Math.Round(n13, 2);
                                multiGainer.txt_J13 = Convert.ToString(j13);
                                multiGainer.txt_N13 = Convert.ToString(n13);


                            }
                            //Convert.ToString(Math.Round(((100* buycount)/3)),0)); 

                        }
                    }
                    if (((100 * salcount) / 3) >= 100)
                    {
                        if (k13 == "BUY" && salcount >= 2)
                        {

                            // in revert call is average is below m6 or above m6  then make both m6 and average same
                            // by making changes in average value try to get the nearest value to m6 so final average value should be equal or nearer to m6
                            // changes done on 2/may/2016
                            // m3 is average
                            double tempm3 = m3;
                            double tempm6 = 0;
                            double testmv = 0;
                            if (m3 != m6 && !(goforrecaculation))
                            {

                                for (double i = m3; i <= m3 + 200; i++)
                                {
                                    tempm3 = i;
                                    tempm6 = (tempm3 + m4 + m5) / 3;
                                    tempm6 = Math.Round(tempm6, 2);
                                    if (tempm3 >= tempm6)
                                    {
                                        // tempm3 = tempm3 - 1;
                                        tempm6 = (tempm3 + m4 + m5) / 3;
                                        tempm6 = Math.Round(tempm6, 2);
                                        testmv = tempm6;
                                        m3 = tempm3;
                                        multiGainer.txt_m3 = m3.ToString(); // added on 20/may/2015
                                        goforrecaculation = true;
                                        //  goto outprint;
                                        goto Recalculate; // label define at the top for recalculating 
                                    }

                                }
                            }
                        outprint:
                            multiGainer.txt_reversecall = "SELL";
                            // according to sunilbhai add the value of 0.00013 of n6 to the n6 value 
                            double intrimn6value = 0;
                            intrimn6value = (0.00013 * n6);
                            n6 = n6 + intrimn6value;
                            n6 = Math.Round(n6, 2);
                            multiGainer.Txt_salesl = Convert.ToString(n6);
                            multiGainer.Txt_salet1 = Convert.ToString(j6);
                            multiGainer.Txt_salet2 = Convert.ToString(i6);
                            multiGainer.Txt_salet3 = Convert.ToString(h6);
                            multiGainer.txt_reverseBAP = Convert.ToString(m3);  // as per output excel it is average or m6 value
                                                                                //multiGainer.txt_possibilities = Convert.ToString(((100 * salcount) / 3));
                                                                                // here we need to change the value of s-bap and r-bap if reverse call is there and 
                                                                                // if trandview is buy then take s-bap take 0.576 and r-bap is 0.876 as per whatapps on 19/may/2015
                            j13 = (j13 * 0.576); //s-bap
                            j13 = Math.Round(j13, 2);
                            n13 = (n13 * 0.876);  // r-bap
                            n13 = Math.Round(n13, 2);
                            multiGainer.txt_J13 = Convert.ToString(j13);
                            multiGainer.txt_N13 = Convert.ToString(n13);

                        }
                    }
                    // if trandview is buy then check for salecount equal or more than 2 then sell is the result



                    // calculation for band view call on 25/april2016
                    // we need to calculate the buy and sell count again for band view call and formula is different
                    // if special case are there then no need for checking for band view call as on 9/may/2015 by suniljoshi on call in mroning
                    if (!specialcase)
                    {

                        int bvcbuycount = 0;
                        int bvcsellcount = 0;
                        string s25 = "";
                        //formula for counting buy count
                        if (j11 == "BUY")
                        {
                            bvcbuycount = 0;
                            bvcsellcount = 0;
                            //IF(T10>N13,E8,F8)
                            if (t10 > n13)
                            {
                                bvcbuycount = bvcbuycount + 1;
                            }
                            else
                            {
                                bvcsellcount = bvcsellcount + 1;
                            }
                            //IF(D10<J6,E8,F8)
                            if (d10 < j6)
                            {
                                bvcbuycount = bvcbuycount + 1;
                            }
                            else
                            {
                                bvcsellcount = bvcsellcount + 1;
                            }
                            //IF(M3<K6,E8,F8)
                            if (m3 < k6)
                            {
                                bvcbuycount = bvcbuycount + 1;
                                s25 = "BUY";
                            }
                            else
                            {
                                bvcsellcount = bvcsellcount + 1;
                                s25 = "SELL";
                            }
                        }


                        // formula for counting sell count 
                        string t25 = "";
                        if (j11 == "SELL")
                        {
                            bvcbuycount = 0;
                            bvcsellcount = 0;

                            //IF(D10<J13,F8,E8)
                            if (d10 < j13)
                            {
                                bvcsellcount = bvcsellcount + 1;
                            }
                            else
                            {
                                bvcbuycount = bvcbuycount + 1;
                            }
                            //=IF(T10>O6,F8,E8)
                            if (t10 > o6)
                            {
                                bvcsellcount = bvcsellcount + 1;
                            }
                            else
                            {
                                bvcbuycount = bvcbuycount + 1;
                            }
                            //IF(M3>N6,F8,E8)
                            if (m3 > n6)
                            {
                                bvcsellcount = bvcsellcount + 1;
                                t25 = "SELL";

                            }
                            else
                            {
                                bvcbuycount = bvcbuycount + 1;
                                t25 = "BUY";
                            }
                        }
                        if (j11 == "BUY" && trandview == "SELL" && !(s25 == "SELL"))
                        {
                            multiGainer.txt_tcreversecall = "BUY";
                            // txt_tcBuysl = Convert.ToString(k6); // commented as per sunilbhai on 4/may/2015
                            multiGainer.txt_tcBuysl = Convert.ToString(i12);
                            multiGainer.txt_tcbuyt1 = Convert.ToString(o6);
                            multiGainer.txt_tcbuyt2 = Convert.ToString(p6);
                            multiGainer.txt_tcbuyt3 = Convert.ToString(q6);

                            // txt_tcpossibilities = Convert.ToString(((100 * bvcbuycount) / 3));
                            multiGainer.Label60 = "BAND VIEW CALL WITH " + Convert.ToString(((100 * bvcbuycount) / 3)) + "% POSSIBILITIES";
                            //HERE BAP IS A S-BAP IF BAND VIEW CALL BUY OR R-BAP IF BAND VIEW CALL SELL
                            multiGainer.txt_BVCBAP = Convert.ToString(j13); // s-bap




                        }
                        if (j11 == "SELL" && trandview == "BUY" && !(t25 == "BUY"))
                        {
                            multiGainer.txt_tcreversecall = "SELL";
                            // Txt_tcsalesl = Convert.ToString(n6);  // after discussion with sunilbhai as on 4/may/2015
                            multiGainer.Txt_tcsalesl = Convert.ToString(o12);
                            multiGainer.Txt_tcsalet1 = Convert.ToString(j6);
                            multiGainer.Txt_tcsalet2 = Convert.ToString(i6);
                            multiGainer.Txt_tcsalet3 = Convert.ToString(h6);
                            multiGainer.Label60 = "BAND VIEW CALL WITH " + Convert.ToString(((100 * bvcsellcount) / 3)) + "% POSSIBILITIES";
                            //txt_tcpossibilities = Convert.ToString(((100 * bvcsellcount) / 3));
                            //HERE BAP IS A S-BAP IF BAND VIEW CALL BUY OR R-BAP IF BAND VIEW CALL SELL
                            multiGainer.txt_BVCBAP = Convert.ToString(n13);  // r-bap

                        }

                    }
                    // end of special case checking on 9/may/2015 as per discussion with sunil joshi in morning on call 

                    #endregion

                    //

                    multiGainer.Result = Convert.ToString(Convert.ToInt32(multiGainer.IDH) * 5);
                    multiGainer.Result = Convert.ToString(Convert.ToDouble(multiGainer.txt_m3) * Convert.ToInt32(multiGainer.IDH));
                    //nt

                    //SWF and LWF
                    double higherSWF = 0, lowerSWF = 0, higherLWF, lowerLWF = 0;

                    if (Convert.ToDouble(multiGainer.txt_t11) > Convert.ToDouble(multiGainer.txt_t10))
                        higherSWF = Convert.ToDouble(multiGainer.txt_t11);
                    if (Convert.ToDouble(multiGainer.txt_t10) > Convert.ToDouble(multiGainer.txt_t11))
                        higherSWF = Convert.ToDouble(multiGainer.txt_t10);

                    if (Convert.ToDouble(multiGainer.txt_d11) < Convert.ToDouble(multiGainer.txt_d10))
                        lowerSWF = Convert.ToDouble(multiGainer.txt_d11);
                    if (Convert.ToDouble(multiGainer.txt_d10) < Convert.ToDouble(multiGainer.txt_d11))
                        lowerSWF = Convert.ToDouble(multiGainer.txt_d10);

                    higherLWF = Convert.ToDouble(multiGainer.txt_q16);
                    lowerLWF = Convert.ToDouble(multiGainer.txt_f19);

                    multiGainer.swf11 = (lowerSWF + ((higherSWF - lowerSWF) * 9.87)).ToString();
                    multiGainer.swf10 = (lowerSWF + ((higherSWF - lowerSWF) * 6.1)).ToString();
                    multiGainer.swf9 = (lowerSWF + ((higherSWF - lowerSWF) * 3.77)).ToString();
                    multiGainer.swf1 = (lowerSWF + ((higherSWF - lowerSWF) * 2.33)).ToString();
                    multiGainer.swf2 = (lowerSWF + ((higherSWF - lowerSWF) * 1.44)).ToString();
                    multiGainer.swf3 = (lowerSWF + ((higherSWF - lowerSWF) * 0.89)).ToString();
                    multiGainer.swf4 = (higherSWF - ((higherSWF - lowerSWF) * 0.34)).ToString();
                    multiGainer.swf5 = (lowerSWF + ((higherSWF - lowerSWF) * 0.34)).ToString();
                    multiGainer.swf6 = (higherSWF - ((higherSWF - lowerSWF) * 0.89)).ToString();
                    multiGainer.swf7 = (higherSWF - ((higherSWF - lowerSWF) * 1.44)).ToString();
                    multiGainer.swf8 = (higherSWF - ((higherSWF - lowerSWF) * 2.33)).ToString();
                    multiGainer.swf12 = (higherSWF - ((higherSWF - lowerSWF) * 3.77)).ToString();
                    multiGainer.swf13 = (higherSWF - ((higherSWF - lowerSWF) * 6.1)).ToString();
                    multiGainer.swf14 = (higherSWF - ((higherSWF - lowerSWF) * 9.87)).ToString();

                    multiGainer.lwf11 = (lowerLWF + ((higherLWF - lowerLWF) * 9.87)).ToString();
                    multiGainer.lwf10 = (lowerLWF + ((higherLWF - lowerLWF) * 6.1)).ToString();
                    multiGainer.lwf9 = (lowerLWF + ((higherLWF - lowerLWF) * 3.77)).ToString();
                    multiGainer.lwf1 = (lowerLWF + ((higherLWF - lowerLWF) * 2.33)).ToString();
                    multiGainer.lwf2 = (lowerLWF + ((higherLWF - lowerLWF) * 1.44)).ToString();
                    multiGainer.lwf3 = (lowerLWF + ((higherLWF - lowerLWF) * 0.89)).ToString();
                    multiGainer.lwf4 = (higherLWF - ((higherLWF - lowerLWF) * 0.34)).ToString();
                    multiGainer.lwf5 = (lowerLWF + ((higherLWF - lowerLWF) * 0.34)).ToString();
                    multiGainer.lwf6 = (higherLWF - ((higherLWF - lowerLWF) * 0.89)).ToString();
                    multiGainer.lwf7 = (higherLWF - ((higherLWF - lowerLWF) * 1.44)).ToString();
                    multiGainer.lwf8 = (higherLWF - ((higherLWF - lowerLWF) * 2.33)).ToString();
                    multiGainer.lwf12 = (higherLWF - ((higherLWF - lowerLWF) * 3.77)).ToString();
                    multiGainer.lwf13 = (higherLWF - ((higherLWF - lowerLWF) * 6.1)).ToString();
                    multiGainer.lwf14 = (higherLWF - ((higherLWF - lowerLWF) * 9.87)).ToString();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                message = "we regret for unable to serve in this case";
                return null;
            }
            // connection();
            // List<SelectListItem> items = new List<SelectListItem>();
            // String sql = "SELECT distinct[symbol] FROM [BhagirathFinCare].[dbo].[MarketPrice] order by [symbol]";
            // SqlCommand cmd1 = new SqlCommand(sql, conn);
            // SqlDataReader rdr = cmd1.ExecuteReader();
            // while (rdr.Read())
            // {
            //     items.Add(new SelectListItem
            //     {
            //         Text = (string)rdr["symbol"],
            //         Value = (string)rdr["symbol"]
            //     });
            // }
            // ViewBag.itemsList = items;
            //// multiGainer.Symbol = items;
            message = "Data Loaded Successfully";
            return multiGainer;
        }

        private string GetSymbolNameFromScriptCode(string scriptCode)
        {
            string symbolName = scriptCode;
            //try
            //{
            //    connection();
            //    string sqlQuery = string.Format("SELECT top 1 Symbol from MarketPrice where ScriptCode='{0}'", scriptCode);
            //    DataTable dt = SqlHelper.ExecuteDataTable(sqlconn, CommandType.Text, sqlQuery);
            //    if (dt != null && dt.Rows.Count > 0)
            //    {
            //        DataRow dr = dt.Rows[0];
            //        symbolName = dr[0].ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ex.LogError(this);
            //    //   ex.LogError(this);
            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}
            return symbolName;
        }
        
        #region TrandView

        public string GetFinalBuyorSell(string Wave1, string Wave2, string Wave3, string Wave4)
        {
            if (Wave1 == Wave2)
            {

                if (Wave1 == "SELL")
                {

                    if (Wave3 == Wave4)
                    {

                        if (Wave3 == "BUY")
                            return "BUY";  //END of condition 3 - END OF LOGIC
                        if (Wave3 == "SELL")
                            return "SELL"; //END of condition 4 - END OF LOGIC
                    }
                    else //if not same go to page 8
                    {
                        if (Wave3 == "BUY")
                            return "SELL"; //END OF LOGIC
                        else if (Wave3 == "SELL")
                        //point 9 and 10 made to be clear on the page 8 
                        {
                            //QUERY YET TO BE RESOLVED
                        }
                    }

                }
                else if (Wave1 == "BUY")
                {
                    if (Wave3 == Wave4)
                    {
                        if (Wave3 == "BUY")
                            return "BUY"; //condition 1- END OF PROCESS 1
                        if (Wave3 == "SELL")
                            return "SELL"; //condition 2 - END OF PROCESS 1
                    }
                }
                else //this is no in the page 1 condition  2
                {
                    if (Wave3 == "SELL")
                    {
                        //compare wave 3 to IDH or lDL
                        //if(daily_result.WAVE3 == daily_result.)
                        //SELL at Rest condition 12
                        //if(near to high)
                        //BUY at Contidion Rest condition 11
                    }
                    if (Wave3 == "SELL")
                        return "BUY"; //end of condition 5
                    if (Wave3 == "SELL")
                    {
                        //go to page 7			
                    }
                }

            } //this is no of - Point Delhi
            else //(no if condition 1 in page 1)
            {
                //go to page 3
                if (Wave1 == "SELL")
                { //yes 
                    if (Wave3 == Wave4)
                    {
                        //go to page 3-3
                        //check(both are buy)
                        if (Wave3 == "BUY" && Wave4 == "BUY")
                            return "BUY"; //- end of condition
                        if (Wave3 == "SELL" && Wave4 == "SELL")
                            return "SELL";// - end of condition
                    }
                    else // not same
                    {
                        //go to page 3-1 
                        //check(wave 3 buy or sell)
                        if (Wave3 == "SELL")
                        {
                            //go to page 3-2
                            //check (cmp with IDL or IDH)
                            // double diffcmpidl = Convert.ToDouble(txt_l8.Text) - Convert.ToDouble(txt_d10.Text);
                            // double diffcmpidh = Convert.ToDouble(txt_l8.Text) - Convert.ToDouble(txt_t10.Text);
                            //if(near to IDL)
                            //SELL at Rest end of condition
                            //if(near to IDH)
                            //BUY at Rest end of conditon
                            //if (diffcmpidl < diffcmpidh)
                            // {
                            // near to idl sell 
                            //    return "SELL";
                            //}
                            //else
                            // {
                            // nearer to idh 
                            //    return "BUY";
                            // }

                        }
                        if (Wave3 == "BUY")
                        {
                            //check(previous day close near to PDL or PDH)

                            //if(near to PDL)
                            //SELL at Rest end of condition
                            //if(near to PDH)
                            //BUY at Rest end of conditon
                        }

                    }

                }
                else
                {// no
                    //go to page 3-11
                    if (Wave3 == Wave4)
                    {
                        //goto page 3-13
                        if (Wave3 == "BUY" && Wave4 == "BUY")
                            return "BUY"; //end of condition
                        if (Wave3 == "SELL" && Wave4 == "SELL")
                            return "SELL"; //end of condition
                    }
                    else
                    {//no
                        //check(wave 3 is buy or sell)
                        if (Wave3 == "BUY")
                        {
                            //go to page 3-12
                            //check(cmp with TDL and TDH)
                            //  double diffcmptdl = Convert.ToDouble(txt_l8.Text) -Convert.ToDouble(txt_d8.Text);
                            // double diffcmptdh = Convert.ToDouble(txt_l8.Text) - Convert.ToDouble(txt_t8.Text) ;
                            //if(Near to TDL)
                            //BUY end of condition		
                            //if(Near to TDH)
                            //SELL end of condtion
                            // if (diffcmptdl < diffcmptdh)
                            // {
                            // nearer to tdl
                            //     return "BUY";
                            // }
                            // else
                            // {
                            // nearer to tdh
                            //    return "SELL";
                            // }
                        }
                        else if (Wave3 == "SELL")
                        {
                            //go to page 3-14			
                            //check(cmp wave to IDH or IDL)
                            // double diffcmpidl = Convert.ToDouble(txt_l8.Text) - Convert.ToDouble(txt_d10.Text);
                            // double diffcmpidh = Convert.ToDouble(txt_l8.Text) - Convert.ToDouble(txt_t10.Text);
                            //if(near to IDL)
                            //SELL at Rest
                            //if(near to IDH)
                            //BUY at Rest
                            // if (diffcmpidl < diffcmpidh)
                            // {
                            // near to idl sell 
                            //    return "SELL";
                            // }
                            //else
                            // {
                            // nearer to idh 
                            //   return "BUY";
                            // }


                        }
                    }
                }
            }
            return "BUY";
        }
        #endregion

        #region Equity - Calculate PDH-PDL, TDH-TDL, CTHD-CTLD
        public abstract PDLPDHSelectedData GetPDLPDHData(DtoRequestModelForCalculate dtoCalculate);
        public abstract TDLTDHSelectedData GetTDLTDHData(DtoRequestModelForCalculate dtoCalculate, decimal pdl, decimal pdh, DateTime pdlpdhDate, string companyName);
        public abstract CTLDCTHDSelectedData GetCTLDCTHDData(DtoRequestModelForCalculate dtoCalculate, decimal currentLow, decimal currentHigh, DateTime currentWorkingDate, string companyName);
        #endregion

    }
}
