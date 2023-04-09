
namespace LipWebApi.wwwroot;
public static class LocolTooth_cb3fab6f_js{
    public static byte[] data = System.Text.Encoding.UTF8.GetBytes(
"""
import{c as B,_ as f,o as a,a as _,w as l,b as u,t as c,d as s,e as i,V,f as d,F as k,r as v,g as F,h as x,i as C,j as b,u as T,k as A,l as L,m as E,n as N,p as h,q as S}from"./index.4f506261.js";
"""
+"""
const D={props:{value:{type:Object,require:!0},actions:{type:Array,require:!1}},computed:{pkg(){return this.value}}},P=B("flex-grow-1","div","VSpacer"),q={style:{margin:"20px","margin-top":"0px"}},w={key:0},j={key:1};
"""
+"""
function I(y,r,o,m,g,t){return a(),_(C,{title:t.pkg.information.name+"@"+t.pkg.version,subtitle:t.pkg.information.description},{default:l(()=>[u("div",q,[u("p",null,"\u4F5C\u8005\uFF1A"+c(t.pkg.information.author),1),t.pkg.information.homepage?(a(),s("p",w," \u4E3B\u9875\uFF1A"+c(t.pkg.information.homepage),1)):i("",!0),t.pkg.information.license?(a(),s("p",j,"\u534F\u8BAE\uFF1A"+c(t.pkg.information.license),1)):i("",!0)]),o.actions?(a(),_(V,{key:0},{default:l(()=>[d(P),(a(!0),s(k,null,v(o.actions,e=>(a(),_(F,{style:{"align-items":"center"},color:e.color+" lighten-1",variant:"text",onClick:e.callback},{default:l(()=>[x(c(e.text),1)]),_:2},1032,["color","onClick"]))),256))]),_:1})):i("",!0)]),_:1},8,["title","subtitle"])}const z=f(D,[["render",I]]),G={key:0,class:"progress-circular"},O={style:{"margin-left":"20px","margin-top":"20px"}},H={style:{"vertical-align":"middle"}},J={class:"tooth-item-card"},K=b({__name:"LocolTooth",setup(y){const r=T(),o=A(),m=L(()=>{var e,p;
"""
+"""
return r.token&&h.getToothList().then(n=>{o.value=n}).catch(n=>{r.message=n}),(p=(e=r.allPath.find(n=>n.value===r.currentPath))==null?void 0:e.name)!=null?p:""}),g=()=>{o.value=null,h.getToothList().then(e=>{o.value=e}).catch(e=>{r.message=e})},t=[{color:"primary",text:"\u68C0\u67E5\u66F4\u65B0",callback:()=>{console.log("\u68C0\u67E5\u66F4\u65B0")}},{color:"secondary",text:"\u5378\u8F7D",callback:()=>{console.log("\u5378\u8F7D")}}];
"""
+"""
return(e,p)=>(a(),s("div",null,[o.value?i("",!0):(a(),s("div",G,[d(E,{size:"64",indeterminate:"",color:"blue-accent-3"})])),u("div",O,[d(F,{variant:"text",onClick:g},{default:l(()=>[x("\u70B9\u51FB\u5237\u65B0 ")]),_:1}),u("span",H," \u5F53\u524D\uFF1A"+c(N(m)),1)]),o.value?(a(!0),s(k,{key:1},v(o.value.packages,n=>(a(),_(S,null,{default:l(()=>[u("div",J,[d(z,{value:n,actions:t},null,8,["value"])])]),_:2},1024))),256)):i("",!0)]))}});
"""
+"""
const Q=f(K,[["__scopeId","data-v-51587382"]]);
"""
+"""
export{Q as default};
"""
+"""


""");
}
                