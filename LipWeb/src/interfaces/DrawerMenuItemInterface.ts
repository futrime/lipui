import type { RouteLocationRaw } from "vue-router";

export default interface DrawerMenuItem {
  title: string | "-";
  icon?: string;
  to?: RouteLocationRaw;
  active?: boolean;
}
