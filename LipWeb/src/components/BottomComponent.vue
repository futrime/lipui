<script setup lang="ts">
import type DrawerMenuItem from "@/interfaces/DrawerMenuItemInterface";
import router from "@/router";
const items: DrawerMenuItem[] = [];
router.getRoutes().forEach((target) => {
  if (target.meta.title) {
    items.push({
      title: target.meta.title as string,
      icon: target.meta.icon as string,
      to: { name: target.name },
    });
  }
});
</script>
<template>
  <v-bottom-navigation nav class="main">
    <template v-for="item in items" :key="item.title">
      <v-divider v-if="item.title === '-'" />
      <template v-else>
        <v-btn
          :disabled="!item.to"
          :prepend-icon="item.icon"
          :title="item.title"
          :to="item.to"
          link
        >
          <span> {{ item.title }}</span>
        </v-btn>
      </template>
    </template>
  </v-bottom-navigation>
</template>
