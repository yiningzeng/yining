﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YiNing.Tools
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// DateTimeHelper
        /// </summary>

        /// <summary>
        /// Unix时间起始时间
        /// </summary>
        public static readonly DateTime StarTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 常用日期格式
        /// </summary>
        public static readonly string CommonDateFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// 周未定义
        /// </summary>
        public static readonly DayOfWeek[] Weekend = { DayOfWeek.Saturday, DayOfWeek.Sunday };

        /// <summary>
        /// 获取从Unix起始时间到给定时间的毫秒数
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long GetMillisecondsSince1970(this DateTime datetime)
        {
            var ts = datetime.Subtract(StarTime);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 获取从Unix起始时间到给定时间的秒数
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static long GetSecondsSince1970(this DateTime datetime)
        {
            var ts = datetime.Subtract(StarTime);
            return (long)ts.TotalSeconds;
        }

        /// <summary>
        /// 最近一天的开始
        /// </summary>
        /// <returns></returns>
        public static DateTime LastDayStart()
        {
            return DateTime.Now.AddDays(-1);
        }

        /// <summary>
        /// 最近一周的开始
        /// </summary>
        /// <returns></returns>
        public static DateTime LastWeekStart()
        {
            return DateTime.Now.AddDays(-7);
        }

        /// <summary>
        /// 最近一月的开始
        /// </summary>
        /// <returns></returns>
        public static DateTime LastMonthStart()
        {
            return DateTime.Now.AddDays(-30);
        }

        /// <summary>
        /// 当天开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime TodayStart()
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 00, 00, 00, 00);
        }

        /// <summary>
        /// 当天结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime TodayEnd()
        {
            return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59, 999);
        }


        /// <summary>
        /// 本周开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime WeekStart()
        {
            DateTime dd = GetWeekFirstDayMon(DateTime.Now);
            return new DateTime(dd.Year, dd.Month, dd.Day, 00, 00, 00, 00);
        }

        /// <summary>
        /// 本周结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime WeekEnd()
        {
            DateTime dd = GetWeekLastDaySun(DateTime.Now);
            return new DateTime(dd.Year, dd.Month, dd.Day, 23, 59, 59, 999);
        }


        /// <summary>
        /// 本月开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime MonthStart()
        {
            DateTime dd = GetFirstDayOfMonth(DateTime.Now);
            return new DateTime(dd.Year, dd.Month, dd.Day, 00, 00, 00, 00);
        }

        /// <summary>
        /// 本月结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime MonthEnd()
        {
            DateTime dd = GetLastDayOfMonth(DateTime.Now);
            return new DateTime(dd.Year, dd.Month, dd.Day, 23, 59, 59, 999);
        }


        /// <summary>
        /// 明天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        /// <summary>
        /// 昨天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime Yesterday(this DateTime date)
        {
            return date.AddDays(-1);
        }

        /// <summary>
        /// 常用日期格式化字符串
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToCommonFormat(this DateTime date)
        {
            return date.ToString(CommonDateFormat);
        }

        /// <summary>
        /// 是否是周未
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime date)
        {
            return Weekend.Any(p => p == date.DayOfWeek);
        }

        /// <summary>
        /// 是否是工作日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsWeekDay(this DateTime date)
        {
            return !date.IsWeekend();
        }

        /// <summary>
        /// 给定月份的第1天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 给定月份的最后1天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime date)
        {
            return date.GetFirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 给定日期所在月份第1个星期几所对应的日期
        /// </summary>
        /// <param name="date">给定日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns>所对应的日期</returns>
        public static DateTime GetFirstWeekDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetFirstDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(1);

            return dt;
        }

        /// <summary>
        /// 给定日期所在月份最后1个星期几所对应的日期
        /// </summary>
        /// <param name="date">给定日期</param>
        /// <param name="dayOfWeek">星期几</param>
        /// <returns>所对应的日期</returns>
        public static DateTime GetLastWeekDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
        {
            var dt = date.GetLastDayOfMonth();
            while (dt.DayOfWeek != dayOfWeek)
                dt = dt.AddDays(-1);

            return dt;
        }

        /// <summary>
        /// 早于给定日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsBefore(this DateTime date, DateTime other)
        {
            return date.CompareTo(other) < 0;
        }

        /// <summary>
        /// 晚于给定日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsAfter(this DateTime date, DateTime other)
        {
            return date.CompareTo(other) > 0;
        }

        /// <summary>
        /// 给定日期最后一刻,精确到23:59:59.999
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndTimeOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        /// <summary>
        ///  给定日期开始一刻,精确到0:0:0.0
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartTimeOfDay(this DateTime date)
        {
            return date.Date;
        }

        /// <summary>
        ///  给定日期的中午,精确到12:0:0.0
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NoonOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 12, 0, 0);
        }

        /// <summary>
        /// 当前日期与给定日期是否是同一天
        /// </summary>
        /// <param name="date">当前日期</param>
        /// <param name="dateToCompare">给定日期</param>
        /// <returns></returns>
        public static bool IsDateEqual(this DateTime date, DateTime dateToCompare)
        {
            return (date.Date == dateToCompare.Date);
        }

        /// <summary>
        /// 判断是否为今天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsToday(this DateTime date)
        {
            return (date.Date == DateTime.Now.Date);
        }

        /// <summary>
        /// 给定日期所在月份共有多少天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetCountDaysOfMonth(this DateTime date)
        {
            return date.GetLastDayOfMonth().Day;
        }

        /// <summary>  
        /// 得到本周第一天(以星期一为第一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekFirstDayMon(DateTime datetime)
        {
            //星期一为第一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);

            //因为是以星期一为第一天，所以要判断weeknow等于0时，要向前推6天。  
            weeknow = (weeknow == 0 ? (7 - 1) : (weeknow - 1));
            int daydiff = (-1) * weeknow;

            //本周第一天  
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay);
        }

        /// <summary>  
        /// 得到本周最后一天(以星期天为最后一天)  
        /// </summary>  
        /// <param name="datetime"></param>  
        /// <returns></returns>  
        public static DateTime GetWeekLastDaySun(DateTime datetime)
        {
            //星期天为最后一天  
            int weeknow = Convert.ToInt32(datetime.DayOfWeek);
            weeknow = (weeknow == 0 ? 7 : weeknow);
            int daydiff = (7 - weeknow);

            //本周最后一天  
            string LastDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(LastDay);
        }
    }
}