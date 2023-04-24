<template>
  <v-card subtitle="已连接到Lip" text="前往其他页面进行操作" variant="tonal" />
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
import { computed, WritableComputedRef } from "vue";
const data = useGlobal();
const currentPath: WritableComputedRef<string> = computed({
  get: () => data.currentPath,
  set: (v: string) => {
    data.currentPath = v;
    data.setMessage(v);
  },
});
</script>
<style lang="scss">
div.main {
  margin: 20px;
}
</style>
