<template>
  <div class="landing-container">
    <div class="background-image"></div>
    <div class="content">
      <h1>欢迎来到gulugulu</h1>
      <!-- <h2></h2> -->
      <button class="go-home-button" @click="goToHome">开启你的漫画之旅 &rarr;</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goToHome = () => {
  router.push('/home')
}
</script>

<style scoped>
.landing-container {
  position: relative;
  width: 100%;
  height: calc(100vh - 60px); /* Adjust height considering the header if needed */
  overflow: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  text-align: center;
}

.background-image {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url('/img/4.jpg'); /* Replace with your image path */
  background-size: cover;
  background-position: center;
  /* filter: brightness(50%) grayscale(30%); */ /* Adjust for desired effect */
  z-index: 0;
}

.landing-container::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6); /* Dark overlay */
  z-index: 0;
}

.content {
  position: relative;
  z-index: 1;
  background-color: rgba(255, 255, 255, 0.15); /* Semi-transparent white background */
  backdrop-filter: blur(10px); /* Apply blur effect */
  -webkit-backdrop-filter: blur(10px); /* For Safari */
  padding: 40px 60px;
  border-radius: 16px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.1);
}

.content h1 {
  font-size: 5em;
  margin-bottom: 100px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.content h2 {
  font-size: 1.5em;
  margin-bottom: 30px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.5);
}

.go-home-button {
  padding: 12px 24px;
  font-size: 1.1em;
  background-color: #2563eb; /* Blue color, similar to your login button */
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition:
    background-color 0.3s ease,
    transform 0.1s ease;
}

.go-home-button:hover {
  background-color: #1a47a1;
}

.go-home-button:active {
  transform: scale(0.98);
}
</style>
