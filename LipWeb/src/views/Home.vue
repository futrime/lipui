<template>
  <v-card subtitle="已连接到Lip" text="前往其他页面进行操作" variant="tonal" />
  <div class="main">
    选择工作目录：
    {{ global.currentPath }}
    <working-path-selector
      v-bind:source="global.allPath"
      v-model:value="currentPath"
    />
  </div>
</template>
<script lang="ts" setup>
import { useGlobal } from "@/store";
import WorkingPathSelector from "@/components/WorkingPathSelector.vue";
import { computed, watch } from "vue";
import api from "@/api";
const global = useGlobal();
const currentPath = computed({
  get: () => global.currentPath,
  set: (v: string) => {
    global.loading = true;
    api
      .setWorkingDirectory(v)
      .then((result) => {
        global.currentPath = result.value;
        global.allPath = result.directories;
      })
      .catch((e) => {
        global.message = e;
      })
      .finally(() => {
        global.loading = false;
      });
  },
});
watch(
  () => global.token,
  (v) => {
    if (v) {
      api
        .getWorkingDirectory()
        .then((result) => {
          global.currentPath = result.current ?? result.directories[0].value;
          global.allPath = result.directories;
        })
        .catch((e) => {
          global.message = e;
        });
    }
  }
);
</script>
<style lang="scss" scoped>
div.main {
  margin: 20px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
