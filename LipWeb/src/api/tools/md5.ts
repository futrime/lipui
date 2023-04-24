import cpt from "crypto-js";
export default function md5(s: string) {
  return cpt.MD5(s).toString();
}
