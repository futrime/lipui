<template>
  <v-hover v-slot="{ isHovering, props }" open-delay="200">
    <v-card
      :elevation="isHovering ? 16 : 2"
      :class="{ 'on-hover': isHovering }"
      :style="{ padding: '15px' }"
      class="mx-auto"
      v-bind="props"
    >
      <v-sheet width="300" class="mx-auto">
        <v-form @submit.prevent>
          <v-text-field
            v-model="firstName"
            :rules="rules"
            label="后端地址"
          ></v-text-field>
          <v-btn type="submit" block class="mt-2">完成</v-btn>
        </v-form>
      </v-sheet>
    </v-card>
  </v-hover>
</template>
<script lang="ts">
import api from "@/api";
export default {
  data: () => ({
    firstName: "",
    rules: [
      (value: string) => {
        try {
          if (!value) return "请填写后端地址";
          //match if start with http
          if (!value.startsWith("http")) return "请填写一个完整的后端http地址";
          api.setAxiosConfig(value);
          if (!api.verifyBackend()) return "后端地址无法访问";
          return true;
        } catch (error) {
          return "错误：" + error;
        }
      },
    ],
  }),
};
</script>
<style lang="scss"></style>
