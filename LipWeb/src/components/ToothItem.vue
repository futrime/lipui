<template>
  <v-card
    :title="pkg.information.name + '@' + pkg.version"
    :subtitle="pkg.information.description"
  >
    <div style="margin: 20px; margin-top: 0px">
      <p>作者：{{ pkg.information.author }}</p>
      <p v-if="pkg.information.homepage">
        主页：{{ pkg.information.homepage }}
      </p>
      <p v-if="pkg.information.license">协议：{{ pkg.information.license }}</p>
    </div>
    <v-card-actions v-if="actions">
      <v-spacer />
      <v-btn
        v-for="item in actions"
        style="align-items: center"
        :color="item.color + ' lighten-1'"
        variant="text"
      >
        {{ item.text }}
      </v-btn>
    </v-card-actions>
  </v-card>
</template>
<script lang="ts">
import { ToothItemInfo } from "@/api/models";
export default {
  props: {
    value: {
      type: Object,
      require: true,
    },
    actions: {
      type: Array<{ color: String; text: String; callback: Function }>,
      require: false,
    },
  },
  computed: {
    pkg() {
      return this.value as ToothItemInfo;
    },
  },
};
</script>
<style></style>
