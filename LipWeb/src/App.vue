<template>
  <v-app :theme="isDark">
    <v-navigation-drawer v-model="drawer" temporary>
      <drawer-component />
    </v-navigation-drawer>
    <v-app-bar>
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-app-bar-title tag="h1"
        >LipWeb - {{ $router.currentRoute.value.meta.title }}</v-app-bar-title
      >
      <v-spacer />
      <app-bar-menu-component />
      <v-progress-linear
        v-show="loading"
        :active="loading"
        :indeterminate="progress === null"
        :model-value="progress !== null ? progress : 0"
        color="blue-accent-3"
      />
    </v-app-bar>
    <v-main>
      <router-view v-slot="{ Component, route }">
        <component :is="Component" :key="route.path" />
      </router-view>
    </v-main>
    <v-overlay v-model="loading" app class="justify-center align-center">
      <v-progress-circular indeterminate size="64" />
    </v-overlay>
    <v-overlay v-model="setup" app class="justify-center align-center">
      <setup-connection-component />
    </v-overlay>
    <bottom-component />
    <v-snackbar
      v-model="snackbarVisibility"
      @update:model-value="onSnackbarChanged"
    >
      {{ snackbarText }}
      <template #actions>
        <v-btn icon @click="onSnackbarChanged">
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </template>
    </v-snackbar>
  </v-app>
  <teleport to="head">
    <meta
      name="theme-color"
      :content="theme.computedThemes.value[isDark].colors.primary"
    />
  </teleport>
</template>
<script lang="ts" setup>
import DrawerComponent from "@/components/DrawerComponent.vue";
import AppBarMenuComponent from "@/components/AppBarMenuComponent.vue";
import BottomComponent from "@/components/BottomComponent.vue";
import SetupConnectionComponent from "@/components/SetupConnectionComponent.vue";
import { useTheme } from "vuetify/lib/framework.mjs";
import { useGlobal, useConfig } from "@/store";
import {
  Ref,
  ref,
  WritableComputedRef,
  computed,
  ComputedRef,
  onMounted,
} from "vue";
const theme = useTheme();
const globalStore = useGlobal();
const configStore = useConfig();
const drawer: Ref<boolean> = ref(false);
const setup: ComputedRef<boolean> = computed(() => {
  return globalStore.apiPath === undefined || globalStore.apiPath === "";
});
const loading: WritableComputedRef<boolean> = computed({
  get: () => globalStore.loading,
  set: (v) => globalStore.setLoading(v),
});
const isDark: ComputedRef<string> = computed(() =>
  configStore._themeDark ? "dark" : "light"
);
onMounted(() => {
  document.title = "LipWeb";
  loading.value = false;
});
// Snackbar
const snackbarVisibility: ComputedRef<boolean> = computed(
  () => globalStore.message !== ""
);
const snackbarText: ComputedRef<string> = computed(() => globalStore.message);
const onSnackbarChanged = () => {
  globalStore.message = "";
};
/** Appbar progressbar value */
const progress: ComputedRef<number | null> = computed(
  () => globalStore.progress
);
</script>
<style lang="scss">
::-webkit-scrollbar {
  display: none; //hide scrollbar
}
</style>
