import {
  createRouter,
  createWebHistory,
  NavigationGuardNext,
  RouteLocationNormalized,
} from "vue-router";
import HomePanel from "@/views/Home.vue";
import { useGlobal } from "@/store";
import { nextTick } from "vue";
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      //https://pictogrammers.com/library/mdi/
      path: "/",
      name: "home",
      meta: {
        title: "主页",
        icon: "mdi-home",
      },
      component: HomePanel,
    },
    {
      path: "/about",
      name: "about",
      meta: {
        title: "关于",
        icon: "mdi-information",
      },
      component: () => import("../views/About.vue"),
    },
    {
      path: "/local",
      name: "local",
      meta: {
        title: "本地包",
        icon: "mdi-package",
      },
      component: () => import("../views/LocolTooth.vue"),
    },
  ],
});
router.beforeEach(
  async (
    _to: RouteLocationNormalized,
    _from: RouteLocationNormalized,
    next: NavigationGuardNext
  ) => {
    const globalStore = useGlobal();
    // Show Loading
    globalStore.loading = true;
    // Hide snack bar
    globalStore.message = "";
    await nextTick();
    next();
  }
);
router.afterEach(() => {
  const globalStore = useGlobal();
  globalStore.loading = false;
});
export default router;
