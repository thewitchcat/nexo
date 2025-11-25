<script setup lang="ts">
const props = defineProps({
  modelValue: Boolean,
  message: { type: String, default: 'Are you sure?' },
})

const emit = defineEmits(['update:modelValue', 'confirm'])
const close = () => emit('update:modelValue', false)

const confirm = () => {
  emit('confirm')
  emit('update:modelValue', false)
}
</script>

<template>
  <div v-if="modelValue" class="dialog-overlay" @click="close"></div>

  <div v-if="modelValue" class="dialog-box">
    <p>{{ message }}</p>

    <div class="dialog-actions">
      <button @click="close">No</button>
      <button @click="confirm">Yes</button>
    </div>
  </div>
</template>

<style scoped>
.dialog-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 98;
}

.dialog-box {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: white;
  padding: 20px;
  border-radius: 6px;
  z-index: 99;
  min-width: 200px;
}

.dialog-actions {
  margin-top: 20px;
  display: flex;
  justify-content: right;
  gap: 10px;
}
</style>
