<template>
  <v-card class="mx-auto">
    <v-list
      :items="items"
      :selected="[value]"
      @update:selected="updateSelected"
    >
      <template #title="{ item: { value: itemvalue, name: name } }">
        <v-list-item-title>
          <v-icon
            icon="mdi-check"
            v-if="itemvalue === value"
            style="opacity: 0.7"
          />
          {{ name }}
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
      type: Array<{ name: String; value: String }>,
      required: true,
    },
  },
  methods: {
    updateSelected(item: unknown[]) {
      const currentPath = (item as string[])[0] ?? "";
      this.$emit("update:value", currentPath);
    },
  },
  data() {},
  computed: {
    items() {
      return this.source.map((item) => {
        return {
          name: item.name,
          value: item.value,
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
