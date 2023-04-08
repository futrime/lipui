<template>
  <v-card subtitle="已连接到Lip" text="前往其他页面进行操作" variant="tonal" />
  <v-card
    v-if="versionLoaded"
    subtitle="版本信息"
    :text="versionInfo"
    variant="tonal"
  />
  <div class="main">
    选择工作目录：{{ data.currentPath }}
    <working-path-selector
      v-bind:source="data.allPath"
      v-model:value="currentPath"
    />
  </div>
</template>
<script lang="ts" setup>
import { useGlobal } from "@/store";
import WorkingPathSelector from "@/components/WorkingPathSelector.vue";
import { computed, ref, WritableComputedRef } from "vue";
import api from "@/api";
const data = useGlobal();
const currentPath: WritableComputedRef<string> = computed({
  get: () => data.currentPath,
  set: (v: string) => {
    data.loading = true;
    api
      .setWorkingDirectory(v)
      .then((result) => {
        data.currentPath = result.value;
      })
      .catch((e) => {
        data.message = e;
      })
      .finally(() => {
        data.loading = false;
      });
  },
});
const versionInfo = ref("加载中...");
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
            versionInfo.value = "核心: " + lip + " 后端: " + backend;
          })
          .catch((e) => {
            versionInfo.value = e;
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
<style lang="scss">
div.main {
  margin: 20px;
}
</style>
