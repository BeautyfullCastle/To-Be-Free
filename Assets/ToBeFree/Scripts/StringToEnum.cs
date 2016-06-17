using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToBeFree
{
    public class StringToEnum : Singleton<StringToEnum>
    {
        public eStartTime ToStartTime(string startTime)
        {
            switch(startTime)
            {
                case "BROKER":
                    return eStartTime.BROKER;
                case "ESCAPE":
                    return eStartTime.ESCAPE;
                case "INFO":
                    return eStartTime.INFO;
                case "INSPECT":
                    return eStartTime.INSPECT;
                case "MOVE":
                    return eStartTime.MOVE;
                case "NOW":
                    return eStartTime.NOW;
                case "QUEST":
                    return eStartTime.QUEST;
                case "REST":
                    return eStartTime.REST;
                case "SHOP":
                    return eStartTime.SHOP;
                case "SPECIALACT":
                    return eStartTime.SPECIALACT;
                case "TEST":
                    return eStartTime.TEST;
                case "WORK":
                    return eStartTime.WORK;
            }
            return eStartTime.NULL;
        }

        public eDuration ToDuration(string duration)
        {
            switch(duration)
            {
                case "DAY":
                    return eDuration.DAY;
                case "EQUIP":
                    return eDuration.EQUIP;
                case "ONCE":
                    return eDuration.ONCE;
                case "PAT_TEST_REST":
                    return eDuration.PAT_TEST_REST;
            }
            return eDuration.NULL;
        }

        public eStat ToStat(string stat)
        {
            switch(stat)
            {
                case "AGILITY":
                    return eStat.AGILITY;
                case "BARGAIN":
                    return eStat.BARGAIN;
                case "FOOD":
                    return eStat.FOOD;
                case "HP":
                    return eStat.HP;
                case "INFO":
                    return eStat.INFO;
                case "LUCK":
                    return eStat.LUCK;
                case "MENTAL":
                    return eStat.MENTAL;
                case "MONEY":
                    return eStat.MONEY;
                case "OBSERVATION":
                    return eStat.OBSERVATION;
                case "PATIENCE":
                    return eStat.PATIENCE;
                case "STRENGTH":
                    return eStat.STRENGTH;
                case "TOTALFOOD":
                    return eStat.TOTALFOOD;
                case "TOTALHP":
                    return eStat.TOTALHP;
                case "TOTALMENTAL":
                    return eStat.TOTALMENTAL;
            }
            return eStat.NULL;
        }

        public eRegion ToRegion(string region)
        {
            switch(region)
            {
                case "ALL":
                    return eRegion.ALL;
                case "AREA":
                    return eRegion.AREA;
                case "CITY":
                    return eRegion.CITY;
            }
            return eRegion.NULL;
        }

        public eCity ToCity(string city)
        {
            switch(city)
            {
                case "BEIJING":
                    return eCity.BEIJING;
                case "CHANGCHUN":
                    return eCity.CHANGCHUN;
                case "CHANGSHA":
                    return eCity.CHANGSHA;
                case "CHENDU":
                    return eCity.CHENDU;
                case "CHONGQING":
                    return eCity.CHONGQING;
                case "DALIAN":
                    return eCity.DALIAN;
                case "DANDONG":
                    return eCity.DANDONG;
                case "FUZHOU":
                    return eCity.FUZHOU;
                case "GUANGZHOU":
                    return eCity.GUANGZHOU;
                case "HAERBIN":
                    return eCity.HAERBIN;
                case "JILIN":
                    return eCity.JILIN;
                case "JINAN":
                    return eCity.JINAN;
                case "KUNMING":
                    return eCity.KUNMING;
                case "LANZHOU":
                    return eCity.LANZHOU;
                case "NANCHANG":
                    return eCity.NANCHANG;
                case "NANJING":
                    return eCity.NANJING;
                case "NANNING":
                    return eCity.NANNING;
                case "NONE":
                    return eCity.NONE;
                case "QINGDAO":
                    return eCity.QINGDAO;
                case "SHANGHAI":
                    return eCity.SHANGHAI;
                case "SHENYANG":
                    return eCity.SHENYANG;
                case "SHIJIAZHUANG":
                    return eCity.SHIJIAZHUANG;
                case "TIANJIN":
                    return eCity.TIANJIN;
                case "WUHAN":
                    return eCity.WUHAN;
                case "XIAN":
                    return eCity.XIAN;
                case "XIANGGANG":
                    return eCity.XIANGGANG;
                case "XINNIG":
                    return eCity.XINNIG;
                case "YANBIAN":
                    return eCity.YANBIAN;
                case "YANTAI":
                    return eCity.YANTAI;
                case "ZHENGZHOU":
                    return eCity.ZHENGZHOU;
            }
            return eCity.NULL;
        }

        public eArea ToArea(string area)
        {
            switch(area)
            {
                case "FUJIANSHENG":
                    return eArea.FUJIANSHENG;
                case "GUANGDONGSHENG":
                    return eArea.GUANGDONGSHENG;
                case "GUANGXISHENG":
                    return eArea.GUANGXISHENG;
                case "HEBEISHENG":
                    return eArea.HEBEISHENG;
                case "HUNANSHENG":
                    return eArea.HUNANSHENG;
                case "JIANGSUSHENG":
                    return eArea.JIANGSUSHENG;
                case "JILINSHENG":
                    return eArea.JILINSHENG;
                case "LIAONINGSHENG":
                    return eArea.LIAONINGSHENG;
                case "NONE":
                    return eArea.NONE;
                case "QINGHAISHENG":
                    return eArea.QINGHAISHENG;
                case "SHANDONGSHENG":
                    return eArea.SHANDONGSHENG;
                case "SHANXISHENG":
                    return eArea.SHANXISHENG;
                case "SICHUANSHENG":
                    return eArea.SICHUANSHENG;
                case "YUNNANSHENG":
                    return eArea.YUNNANSHENG;
                case "MONGOLIA":
                    return eArea.MONGOLIA;
                case "SOUTHEAST_ASIA":
                    return eArea.SOUTHEAST_ASIA;
            }
            return eArea.NULL;
        }

        public eEventAction ToEventAction(string action)
        {
            switch(action)
            {
                case "DETENTION":
                    return eEventAction.DETENTION;
                case "ESCAPE":
                    return eEventAction.ESCAPE;
                case "GLOBAL":
                    return eEventAction.GLOBAL;
                case "INSPECT":
                    return eEventAction.INSPECT;
                case "MOVE":
                    return eEventAction.MOVE;
                case "WORK":
                    return eEventAction.WORK;
            }
            return eEventAction.NULL;
        }
    }
}
