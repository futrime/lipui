<template>
  <v-card
    v-if="versionLoaded"
    subtitle="核心版本："
    :text="versionInfoCore"
    variant="tonal"
  />
  <v-card
    v-if="versionLoaded"
    subtitle="后端版本："
    :text="versionInfoBackend"
    variant="tonal"
  />
</template>
<script setup lang="ts">
import api from "@/api";
import { useGlobal } from "@/store";
import { computed, ref } from "vue";
const data = useGlobal();
const versionInfoCore = ref("加载中...");
const versionInfoBackend = ref("加载中...");
const versionLoaded = computed(() => {
  if (data.token) {
    api
      .getWorkingDirectory()
      .then(({ directories, current }) => {
        data.allPath = directories;
        data.currentPath = current ?? directories[0].value;
        api
          .getVersion()
          .then(({ lip, backend }) => {
            versionInfoCore.value = lip;
            versionInfoBackend.value = backend;
          })
          .catch((e) => {
            data.message = e;
            versionInfoCore.value = "加载失败";
            versionInfoBackend.value = "加载失败";
          });
      })
      .catch((e) => {
        data.message = e;
      });
    return true;
  }
  return false;
});
</script>
