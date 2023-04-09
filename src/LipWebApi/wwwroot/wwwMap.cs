
using System;
using System.Collections.Generic;
using System.Text;

namespace LipWebApi.wwwroot;
internal static class wwwMap
{
    internal static Dictionary<string, Func<byte[]>> dict = new()
    {
        {"favicon.ico", () => favicon_ico.data},
{"index.html", () => index_html.data},
{"assets/About.dd40c6e9.js", () => About_dd40c6e9_js.data},
{"assets/index.4f506261.js", () => index_4f506261_js.data},
{"assets/index.cce171ef.css", () => index_cce171ef_css.data},
{"assets/LocolTooth.ca585f31.css", () => LocolTooth_ca585f31_css.data},
{"assets/LocolTooth.cb3fab6f.js", () => LocolTooth_cb3fab6f_js.data},
{"assets/materialdesignicons-webfont.48d3eec6.woff", () => materialdesignicons_webfont_48d3eec6_woff.data},
{"assets/materialdesignicons-webfont.861aea05.eot", () => materialdesignicons_webfont_861aea05_eot.data},
{"assets/materialdesignicons-webfont.bd725a7a.ttf", () => materialdesignicons_webfont_bd725a7a_ttf.data},
{"assets/materialdesignicons-webfont.e52d60f6.woff2", () => materialdesignicons_webfont_e52d60f6_woff2.data},
{"assets/webfontloader.b777d690.js", () => webfontloader_b777d690_js.data},

    };
}
    