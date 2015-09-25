using System;
using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.ViewModels
{
    /// <summary>
    /// Setting view model
    /// </summary>
    public class SettingsVM
    {
        /// <summary>
        /// Blog settings
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Time zones
        /// </summary>
        public List<SelectOption> TimeZones
        {
            get
            {
                var zones = new List<SelectOption>();
                foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
                {
                    zones.Add(new SelectOption { OptionName = zone.DisplayName, OptionValue = zone.Id });
                }
                return zones;
            }
        }
        /// <summary>
        /// Feed options
        /// </summary>
        public List<SelectOption> FeedOptions
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "RSS 2.0", OptionValue = "Rss" });
                options.Add(new SelectOption { OptionName = "Atom 1.0", OptionValue = "Atom" });
                return options;
            }
        }
        /// <summary>
        /// Commnents per page
        /// </summary>
        public List<SelectOption> CommentsPerPageOptions
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "5", OptionValue = "5" });
                options.Add(new SelectOption { OptionName = "10", OptionValue = "10" });
                options.Add(new SelectOption { OptionName = "15", OptionValue = "15" });
                options.Add(new SelectOption { OptionName = "20", OptionValue = "20" });
                options.Add(new SelectOption { OptionName = "50", OptionValue = "50" });
                return options;
            }
        }
        /// <summary>
        /// Facebook languages
        /// </summary>
        public List<SelectOption> FacebookLanguages
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "Afrikaans", OptionValue = "af_ZA" });
                options.Add(new SelectOption { OptionName = "Akan", OptionValue = "ak_GH" });
                options.Add(new SelectOption { OptionName = "Amharic", OptionValue = "am_ET" });
                options.Add(new SelectOption { OptionName = "Arabic", OptionValue = "ar_AR" });
                options.Add(new SelectOption { OptionName = "Assamese", OptionValue = "as_IN" });
                options.Add(new SelectOption { OptionName = "Aymara", OptionValue = "ay_BO" });
                options.Add(new SelectOption { OptionName = "Azerbaijani", OptionValue = "az_AZ" });
                options.Add(new SelectOption { OptionName = "Belarusian", OptionValue = "be_BY" });
                options.Add(new SelectOption { OptionName = "Bulgarian", OptionValue = "bg_BG" });
                options.Add(new SelectOption { OptionName = "Bengali", OptionValue = "bn_IN" });
                options.Add(new SelectOption { OptionName = "Breton", OptionValue = "br_FR" });
                options.Add(new SelectOption { OptionName = "Bosnian", OptionValue = "bs_BA" });
                options.Add(new SelectOption { OptionName = "Catalan", OptionValue = "ca_ES" });
                options.Add(new SelectOption { OptionName = "SoraniKurdish", OptionValue = "cb_IQ" });
                options.Add(new SelectOption { OptionName = "Cherokee", OptionValue = "ck_US" });
                options.Add(new SelectOption { OptionName = "Corsican", OptionValue = "co_FR" });
                options.Add(new SelectOption { OptionName = "Czech", OptionValue = "cs_CZ" });
                options.Add(new SelectOption { OptionName = "Cebuano", OptionValue = "cx_PH" });
                options.Add(new SelectOption { OptionName = "Welsh", OptionValue = "cy_GB" });
                options.Add(new SelectOption { OptionName = "Danish", OptionValue = "da_DK" });
                options.Add(new SelectOption { OptionName = "German", OptionValue = "de_DE" });
                options.Add(new SelectOption { OptionName = "Greek", OptionValue = "el_GR" });
                options.Add(new SelectOption { OptionName = "English(UK)", OptionValue = "en_GB" });
                options.Add(new SelectOption { OptionName = "English(India)", OptionValue = "en_IN" });
                options.Add(new SelectOption { OptionName = "English(Pirate)", OptionValue = "en_PI" });
                options.Add(new SelectOption { OptionName = "English(UpsideDown)", OptionValue = "en_UD" });
                options.Add(new SelectOption { OptionName = "English(US)", OptionValue = "en_US" });
                options.Add(new SelectOption { OptionName = "Esperanto", OptionValue = "eo_EO" });
                options.Add(new SelectOption { OptionName = "Spanish(Chile)", OptionValue = "es_CL" });
                options.Add(new SelectOption { OptionName = "Spanish(Colombia)", OptionValue = "es_CO" });
                options.Add(new SelectOption { OptionName = "Spanish(Spain)", OptionValue = "es_ES" });
                options.Add(new SelectOption { OptionName = "Spanish", OptionValue = "es_LA" });
                options.Add(new SelectOption { OptionName = "Spanish(Mexico)", OptionValue = "es_MX" });
                options.Add(new SelectOption { OptionName = "Spanish(Venezuela)", OptionValue = "es_VE" });
                options.Add(new SelectOption { OptionName = "Estonian", OptionValue = "et_EE" });
                options.Add(new SelectOption { OptionName = "Basque", OptionValue = "eu_ES" });
                options.Add(new SelectOption { OptionName = "Persian", OptionValue = "fa_IR" });
                options.Add(new SelectOption { OptionName = "LeetSpeak", OptionValue = "fb_LT" });
                options.Add(new SelectOption { OptionName = "Fulah", OptionValue = "ff_NG" });
                options.Add(new SelectOption { OptionName = "Finnish", OptionValue = "fi_FI" });
                options.Add(new SelectOption { OptionName = "Faroese", OptionValue = "fo_FO" });
                options.Add(new SelectOption { OptionName = "French(Canada)", OptionValue = "fr_CA" });
                options.Add(new SelectOption { OptionName = "French(France)", OptionValue = "fr_FR" });
                options.Add(new SelectOption { OptionName = "Frisian", OptionValue = "fy_NL" });
                options.Add(new SelectOption { OptionName = "Irish", OptionValue = "ga_IE" });
                options.Add(new SelectOption { OptionName = "Galician", OptionValue = "gl_ES" });
                options.Add(new SelectOption { OptionName = "Guarani", OptionValue = "gn_PY" });
                options.Add(new SelectOption { OptionName = "Gujarati", OptionValue = "gu_IN" });
                options.Add(new SelectOption { OptionName = "ClassicalGreek", OptionValue = "gx_GR" });
                options.Add(new SelectOption { OptionName = "Hausa", OptionValue = "ha_NG" });
                options.Add(new SelectOption { OptionName = "Hebrew", OptionValue = "he_IL" });
                options.Add(new SelectOption { OptionName = "Hindi", OptionValue = "hi_IN" });
                options.Add(new SelectOption { OptionName = "Croatian", OptionValue = "hr_HR" });
                options.Add(new SelectOption { OptionName = "Hungarian", OptionValue = "hu_HU" });
                options.Add(new SelectOption { OptionName = "Armenian", OptionValue = "hy_AM" });
                options.Add(new SelectOption { OptionName = "Indonesian", OptionValue = "id_ID" });
                options.Add(new SelectOption { OptionName = "Igbo", OptionValue = "ig_NG" });
                options.Add(new SelectOption { OptionName = "Icelandic", OptionValue = "is_IS" });
                options.Add(new SelectOption { OptionName = "Italian", OptionValue = "it_IT" });
                options.Add(new SelectOption { OptionName = "Japanese", OptionValue = "ja_JP" });
                options.Add(new SelectOption { OptionName = "Japanese(Kansai)", OptionValue = "ja_KS" });
                options.Add(new SelectOption { OptionName = "Javanese", OptionValue = "jv_ID" });
                options.Add(new SelectOption { OptionName = "Georgian", OptionValue = "ka_GE" });
                options.Add(new SelectOption { OptionName = "Kazakh", OptionValue = "kk_KZ" });
                options.Add(new SelectOption { OptionName = "Khmer", OptionValue = "km_KH" });
                options.Add(new SelectOption { OptionName = "Kannada", OptionValue = "kn_IN" });
                options.Add(new SelectOption { OptionName = "Korean", OptionValue = "ko_KR" });
                options.Add(new SelectOption { OptionName = "Kurdish(Kurmanji)", OptionValue = "ku_TR" });
                options.Add(new SelectOption { OptionName = "Latin", OptionValue = "la_VA" });
                options.Add(new SelectOption { OptionName = "Ganda", OptionValue = "lg_UG" });
                options.Add(new SelectOption { OptionName = "Limburgish", OptionValue = "li_NL" });
                options.Add(new SelectOption { OptionName = "Lingala", OptionValue = "ln_CD" });
                options.Add(new SelectOption { OptionName = "Lao", OptionValue = "lo_LA" });
                options.Add(new SelectOption { OptionName = "Lithuanian", OptionValue = "lt_LT" });
                options.Add(new SelectOption { OptionName = "Latvian", OptionValue = "lv_LV" });
                options.Add(new SelectOption { OptionName = "Malagasy", OptionValue = "mg_MG" });
                options.Add(new SelectOption { OptionName = "Macedonian", OptionValue = "mk_MK" });
                options.Add(new SelectOption { OptionName = "Malayalam", OptionValue = "ml_IN" });
                options.Add(new SelectOption { OptionName = "Mongolian", OptionValue = "mn_MN" });
                options.Add(new SelectOption { OptionName = "Marathi", OptionValue = "mr_IN" });
                options.Add(new SelectOption { OptionName = "Malay", OptionValue = "ms_MY" });
                options.Add(new SelectOption { OptionName = "Maltese", OptionValue = "mt_MT" });
                options.Add(new SelectOption { OptionName = "Burmese", OptionValue = "my_MM" });
                options.Add(new SelectOption { OptionName = "Norwegian(bokmal)", OptionValue = "nb_NO" });
                options.Add(new SelectOption { OptionName = "Ndebele", OptionValue = "nd_ZW" });
                options.Add(new SelectOption { OptionName = "Nepali", OptionValue = "ne_NP" });
                options.Add(new SelectOption { OptionName = "Dutch(België)", OptionValue = "nl_BE" });
                options.Add(new SelectOption { OptionName = "Dutch", OptionValue = "nl_NL" });
                options.Add(new SelectOption { OptionName = "Norwegian(nynorsk)", OptionValue = "nn_NO" });
                options.Add(new SelectOption { OptionName = "Chewa", OptionValue = "ny_MW" });
                options.Add(new SelectOption { OptionName = "Oriya", OptionValue = "or_IN" });
                options.Add(new SelectOption { OptionName = "Punjabi", OptionValue = "pa_IN" });
                options.Add(new SelectOption { OptionName = "Polish", OptionValue = "pl_PL" });
                options.Add(new SelectOption { OptionName = "Pashto", OptionValue = "ps_AF" });
                options.Add(new SelectOption { OptionName = "Portuguese(Brazil)", OptionValue = "pt_BR" });
                options.Add(new SelectOption { OptionName = "Portuguese(Portugal)", OptionValue = "pt_PT" });
                options.Add(new SelectOption { OptionName = "Quechua", OptionValue = "qu_PE" });
                options.Add(new SelectOption { OptionName = "Romansh", OptionValue = "rm_CH" });
                options.Add(new SelectOption { OptionName = "Romanian", OptionValue = "ro_RO" });
                options.Add(new SelectOption { OptionName = "Russian", OptionValue = "ru_RU" });
                options.Add(new SelectOption { OptionName = "Kinyarwanda", OptionValue = "rw_RW" });
                options.Add(new SelectOption { OptionName = "Sanskrit", OptionValue = "sa_IN" });
                options.Add(new SelectOption { OptionName = "Sardinian", OptionValue = "sc_IT" });
                options.Add(new SelectOption { OptionName = "NorthernSámi", OptionValue = "se_NO" });
                options.Add(new SelectOption { OptionName = "Sinhala", OptionValue = "si_LK" });
                options.Add(new SelectOption { OptionName = "Slovak", OptionValue = "sk_SK" });
                options.Add(new SelectOption { OptionName = "Slovenian", OptionValue = "sl_SI" });
                options.Add(new SelectOption { OptionName = "Shona", OptionValue = "sn_ZW" });
                options.Add(new SelectOption { OptionName = "Somali", OptionValue = "so_SO" });
                options.Add(new SelectOption { OptionName = "Albanian", OptionValue = "sq_AL" });
                options.Add(new SelectOption { OptionName = "Serbian", OptionValue = "sr_RS" });
                options.Add(new SelectOption { OptionName = "Swedish", OptionValue = "sv_SE" });
                options.Add(new SelectOption { OptionName = "Swahili", OptionValue = "sw_KE" });
                options.Add(new SelectOption { OptionName = "Syriac", OptionValue = "sy_SY" });
                options.Add(new SelectOption { OptionName = "Silesian", OptionValue = "sz_PL" });
                options.Add(new SelectOption { OptionName = "Tamil", OptionValue = "ta_IN" });
                options.Add(new SelectOption { OptionName = "Telugu", OptionValue = "te_IN" });
                options.Add(new SelectOption { OptionName = "Tajik", OptionValue = "tg_TJ" });
                options.Add(new SelectOption { OptionName = "Thai", OptionValue = "th_TH" });
                options.Add(new SelectOption { OptionName = "Turkmen", OptionValue = "tk_TM" });
                options.Add(new SelectOption { OptionName = "Filipino", OptionValue = "tl_PH" });
                options.Add(new SelectOption { OptionName = "Klingon", OptionValue = "tl_ST" });
                options.Add(new SelectOption { OptionName = "Turkish", OptionValue = "tr_TR" });
                options.Add(new SelectOption { OptionName = "Tatar", OptionValue = "tt_RU" });
                options.Add(new SelectOption { OptionName = "Tamazight", OptionValue = "tz_MA" });
                options.Add(new SelectOption { OptionName = "Ukrainian", OptionValue = "uk_UA" });
                options.Add(new SelectOption { OptionName = "Urdu", OptionValue = "ur_PK" });
                options.Add(new SelectOption { OptionName = "Uzbek", OptionValue = "uz_UZ" });
                options.Add(new SelectOption { OptionName = "Vietnamese", OptionValue = "vi_VN" });
                options.Add(new SelectOption { OptionName = "Wolof", OptionValue = "wo_SN" });
                options.Add(new SelectOption { OptionName = "Xhosa", OptionValue = "xh_ZA" });
                options.Add(new SelectOption { OptionName = "Yiddish", OptionValue = "yi_DE" });
                options.Add(new SelectOption { OptionName = "Yoruba", OptionValue = "yo_NG" });
                options.Add(new SelectOption { OptionName = "SimplifiedChinese(China)", OptionValue = "zh_CN" });
                options.Add(new SelectOption { OptionName = "TraditionalChinese(HongKong)", OptionValue = "zh_HK" });
                options.Add(new SelectOption { OptionName = "TraditionalChinese(Taiwan)", OptionValue = "zh_TW" });
                options.Add(new SelectOption { OptionName = "Zulu", OptionValue = "zu_ZA" });
                options.Add(new SelectOption { OptionName = "Zazaki", OptionValue = "zz_TR" });
                return options;
            }
        }
        /// <summary>
        /// Closed days options
        /// </summary>
        public List<SelectOption> CloseDaysOptions
        {
            get
            {
                var options = new List<SelectOption>();
                options.Add(new SelectOption { OptionName = "Never" , OptionValue = "0" });
                options.Add(new SelectOption { OptionName = "1", OptionValue = "1" });
                options.Add(new SelectOption { OptionName = "2", OptionValue = "2" });
                options.Add(new SelectOption { OptionName = "3", OptionValue = "3" });
                options.Add(new SelectOption { OptionName = "7", OptionValue = "7" });
                options.Add(new SelectOption { OptionName = "10", OptionValue = "10" });
                options.Add(new SelectOption { OptionName = "14", OptionValue = "14" });
                options.Add(new SelectOption { OptionName = "21", OptionValue = "21" });
                options.Add(new SelectOption { OptionName = "30", OptionValue = "30" });
                options.Add(new SelectOption { OptionName = "60", OptionValue = "60" });
                options.Add(new SelectOption { OptionName = "90", OptionValue = "90" });
                options.Add(new SelectOption { OptionName = "180", OptionValue = "180" });
                options.Add(new SelectOption { OptionName = "365", OptionValue = "365" });
                return options;
            }
        }

        //public List<SelectOption> FeedOptions
        //{
        //    get
        //    {
        //        var options = new List<SelectOption>();
        //        options.Add(new SelectOption { OptionName = "", OptionValue = "" });
        //        return options;
        //    }
        //}

    }
}
