<template>
  <v-card class="mx-auto">
    <v-list
      :items="items"
      :selected="[currentPath]"
      @update:selected="updateSelected"
    >
      <template #title="{ item: { value: value } }">
        <v-list-item-title>
          <v-icon
            icon="mdi-check"
            v-if="value === currentPath"
            style="opacity: 0.7"
          />{{ value }}
        </v-list-item-title>
      </template>
    </v-list>
  </v-card>
</template>
<script lang="ts">
export default {
  props: {
    value: {
      type: String,
      required: true,
    },
    source: {
      type: Array<String>,
      required: true,
    },
  },
  methods: {
    updateSelected(item: unknown[]) {
      this.currentPath = (item as string[])[0] ?? "";
      this.$emit("update:value", this.currentPath);
    },
  },
  data() {
    return {
      currentPath: this.value,
    };
  },
  computed: {
    items() {
      return this.source.map((item) => {
        return {
          value: item,
        };
      });
    },
  },
  emits: ["update:value"],
};
</script>
<style lang="scss" scoped>
.v-list {
  // set item height
  padding-left: 8px;
  padding-right: 8px;
}
</style>
