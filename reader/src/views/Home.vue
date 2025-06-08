<template>
  <div class="home-container">
    <div class="swiper-wrap">
      <swiper
        :modules="[Navigation]"
        :slides-per-view="3"
        :space-between="28"
        :loop="true"
        :loopFillGroupWithBlank="true"
        navigation
        class="custom-swiper"
      >
        <swiper-slide
          v-for="comic in comics"
          :key="comic.id"
          @click="goToDetail(comic.id)"
          class="carousel-card"
        >
          <img :src="comic.cover" class="cover-img" />
          <div class="info">
            <div class="status">{{ comic.status }}</div>
            <h2>{{ comic.title }}</h2>
            <p class="desc">{{ comic.desc }}</p>
            <div class="tags">
              <span v-for="tag in comic.tags" :key="tag">{{ tag }}</span>
            </div>
          </div>
        </swiper-slide>
      </swiper>
    </div>
    <!-- Most Viewed Section -->
    <div class="most-viewed-section">
      <div class="most-viewed-header">
        <h2 class="most-viewed-title">最多观看</h2>
        <div class="most-viewed-tabs">
          <span class="tab active">今日</span>
          <span class="tab">本周</span>
          <span class="tab">本月</span>
        </div>
      </div>
      <div class="most-viewed-list">
        <div v-for="(comic, idx) in mostViewed" :key="comic.id" class="most-viewed-card">
          <div class="rank-badge-capsule-vertical">{{ idx + 1 }}</div>
          <img :src="comic.cover" class="most-viewed-img" />
          <div class="most-viewed-name">{{ comic.title }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Swiper, SwiperSlide } from 'swiper/vue'
import { Navigation } from 'swiper/modules'
import 'swiper/css'
import 'swiper/css/navigation'
import { useRouter } from 'vue-router'

const router = useRouter()
const comics = [
  {
    id: '1ef6ddce-7930-45ae-a335-9a45604b99f7',
    status: 'Releasing',
    title: 'Kingdom',
    desc: 'During the Warring States period in China, Xin and Piao are two...',
    cover:
      'https://uploads.mangadex.org/covers/1ef6ddce-7930-45ae-a335-9a45604b99f7/4269284c-7d26-41a8-86d5-48b64e17323d.jpg',
    tags: ['Action', 'Drama', 'Military'],
  },
  {
    id: '4141c5dc-c525-4df5-afd7-cc7d192a832f',
    status: 'Releasing',
    title: 'Blue Lock',
    desc: 'Yoichi Isagi lost the opportunity to go to the national high school championships because ...',
    cover:
      'https://uploads.mangadex.org/covers/4141c5dc-c525-4df5-afd7-cc7d192a832f/a11a788a-400f-4df0-8b35-2f17123ab879.jpg',
    tags: ['Action', 'Comedy', 'Shounen'],
  },
  {
    id: '48c8ad67-059f-4ca5-b7f4-729fdc72c154',
    status: 'ongoing',
    title: 'Lanxi Zhen',
    desc: 'Lanxi Zhen tells the story of Laojun, Xuanli, and Qingning during the war. It shows a different world of demons and gods....',
    cover:
      'https://uploads.mangadex.org/covers/48c8ad67-059f-4ca5-b7f4-729fdc72c154/3940ba21-144a-42da-a579-7d38097d719c.jpg',
    tags: ['Action', 'Shounen', 'Super Power'],
  },
  {
    id: 'a460ab18-22c1-47eb-a08a-9ee85fe37ec8',
    status: 'Completed',
    title: 'Bleach',
    desc: 'Ichigo Kurosaki has always been able to see ghosts...',
    cover:
      'https://uploads.mangadex.org/covers/a460ab18-22c1-47eb-a08a-9ee85fe37ec8/7c8f9203-2b82-41f2-beb3-e7fb00e151e2.jpg',
    tags: ['Action', 'Shounen', 'Supernatural'],
  },
  {
    id: 'a2c1d849-af05-4bbc-b2a7-866ebb10331f',
    status: 'ongoing',
    title: 'One Piece (Official Colored)',
    desc: 'Gol D. Roger, a man referred to as the "Pirate King," is set to be executed by the World Government...',
    cover:
      'https://uploads.mangadex.org/covers/a2c1d849-af05-4bbc-b2a7-866ebb10331f/da0341d8-5526-452c-8bd3-dc8e3cd89f99.jpg',
    tags: ['Action', 'Shounen', 'Supernatural'],
  },
]

const mostViewed = [
  {
    id: 'a1c7c817-4e59-43b7-9365-09675a149a6f',
    title: 'One Piece',
    cover:
      'https://uploads.mangadex.org/covers/a1c7c817-4e59-43b7-9365-09675a149a6f/249fa95b-2214-4ae3-a8f7-77338fe34542.png',
  },
  {
    id: 'bluelock',
    title: 'Blue Lock',
    cover:
      'https://uploads.mangadex.org/covers/4141c5dc-c525-4df5-afd7-cc7d192a832f/a11a788a-400f-4df0-8b35-2f17123ab879.jpg',
  },
  {
    id: 'chainsawman',
    title: 'Chainsaw Man',
    cover:
      'https://uploads.mangadex.org/covers/a77742b1-befd-49a4-bff5-1ad4e6b0ef7b/bf31b6c3-9075-4c1e-95be-b6a38ffed10f.jpg',
  },
  {
    id: 'fragrant',
    title: 'The Fragrant Flower Blooms With Dignity',
    cover:
      'https://uploads.mangadex.org/covers/418791c0-35cf-4f87-936b-acd9cddf0989/e248ccb3-e923-4523-b795-bdd26bba66f8.jpg',
  },
  {
    id: 'gachiakuta',
    title: 'Gachiakuta',
    cover:
      'https://uploads.mangadex.org/covers/192aa767-2479-42c1-9780-8d65a2efd36a/20fd574a-5c79-4077-ad73-576080251ca8.jpg',
  },
  {
    id: 'sakamoto',
    title: 'Sakamoto Days',
    cover:
      'https://uploads.mangadex.org/covers/9d9b04ad-9a83-49f4-8ae4-a9a3780fe9c0/9b0dd6a1-8021-4182-a191-1c87027647a9.jpg',
  },
  {
    id: '98b0d83a-f3c8-4677-ab9e-1a3576654b8f',
    title: 'Record of Ragnarok',
    cover:
      'https://uploads.mangadex.org/covers/98b0d83a-f3c8-4677-ab9e-1a3576654b8f/3730fe81-548f-462f-b994-c17e7f42b5d0.jpg',
  },
]

function goToDetail(id: string) {
  router.push(`/comic/${id}`)
}
</script>

<style scoped>
.home-container {
  max-width: 1440px;
  margin: 0 auto;
  padding: 32px 32px 0 32px;
  box-sizing: border-box;
}

.home-container::before {
  content: none;
}

.swiper-wrap {
  width: 100%;
  margin: 0 auto 38px auto;
  box-sizing: border-box;
  background: rgba(30, 38, 60, 0.92);
  border-radius: 22px;
  box-shadow: 0 4px 32px 0 rgba(0, 0, 0, 0.1);
  padding: 24px 0 24px 0;
  position: relative;
  z-index: 1;
}

.custom-swiper {
  width: 100%;
  margin: 0;
  box-sizing: border-box;
  padding: 0 24px;
}

.carousel-card {
  width: 100%;
  margin: 0;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(8px);
  -webkit-backdrop-filter: blur(8px);
  border-radius: 16px;
  box-shadow: 0 4px 18px rgba(0, 0, 0, 0.13);
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px 12px 10px 12px;
  min-height: 240px;
  max-width: 320px;
  height: 100%;
  cursor: pointer;
  transition:
    transform 0.22s cubic-bezier(0.4, 2, 0.6, 1),
    box-shadow 0.22s;
  position: relative;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.carousel-card::before {
  content: none;
}

.carousel-card:hover {
  transform: scale(1.045) translateY(-8px) rotateZ(-0.5deg);
  box-shadow: 0 10px 32px rgba(0, 0, 0, 0.19);
  z-index: 2;
}

.cover-img {
  width: 100%;
  max-width: 200px;
  aspect-ratio: 3/4;
  height: auto;
  border-radius: 10px;
  object-fit: contain;
  margin-bottom: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.18);
  background: #232c43;
  display: block;
}

.info {
  color: #fff;
  flex: 1;
  display: flex;
  flex-direction: column;
  width: 100%;
  align-items: flex-start;
}

.status {
  color: #c7cfe6;
  font-size: 0.98rem;
  margin-bottom: 2px;
  font-weight: 400;
}

.info h2 {
  margin: 0 0 8px 0;
  font-size: 1.18rem;
  font-weight: 700;
  line-height: 1.2;
  width: 100%;
  word-break: break-all;
}

.desc {
  font-size: 0.98rem;
  color: #b7bdd6;
  margin-bottom: 10px;
  white-space: pre-line;
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.tags {
  margin-top: auto;
}

.tags span {
  display: inline-block;
  background: #283652;
  color: #ffd04b;
  font-size: 0.92rem;
  border-radius: 11px;
  padding: 2px 13px;
  margin-right: 10px;
  margin-bottom: 5px;
}

.most-viewed-section {
  margin: 38px auto 0 auto;
  padding: 0;
  width: 100%;
  background: rgba(30, 38, 60, 0.92);
  border-radius: 22px;
  box-shadow: 0 4px 32px 0 rgba(0, 0, 0, 0.1);
  padding: 28px 0 32px 0;
}

.most-viewed-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 18px;
  padding: 0 2.5vw;
}

.most-viewed-title {
  color: #c7cfe6;
  font-size: 2rem;
  font-weight: 700;
  margin: 0;
}

.most-viewed-tabs {
  display: flex;
  gap: 18px;
  font-size: 1.1rem;
  color: #7e8ba3;
}

.tab {
  cursor: pointer;
  padding: 2px 18px;
  border-radius: 16px;
  transition:
    background 0.18s,
    color 0.18s;
}

.tab.active {
  color: #fff;
  font-weight: bold;
  background: #2563eb;
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.13);
}

.most-viewed-list {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 24px;
  padding: 0 2.5vw 10px 2.5vw;
}

.most-viewed-card {
  background: linear-gradient(135deg, #232c43 60%, #253144 100%);
  border-radius: 18px;
  width: 100%;
  min-width: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  padding-bottom: 16px;
  transition:
    transform 0.22s cubic-bezier(0.4, 2, 0.6, 1),
    box-shadow 0.22s;
  cursor: pointer;
}

.most-viewed-card:hover {
  transform: scale(1.06) translateY(-7px) rotateZ(-0.5deg);
  box-shadow: 0 10px 32px rgba(0, 0, 0, 0.19);
  z-index: 2;
}

.most-viewed-img {
  width: 100%;
  height: 240px;
  object-fit: cover;
  border-radius: 12px 12px 0 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.13);
}

.rank-badge-capsule-vertical {
  position: absolute;
  top: 12px;
  left: 12px;
  background: linear-gradient(180deg, rgba(0, 0, 0, 0.5) 60%, rgba(0, 0, 0, 0.3) 100%);
  color: #fff;
  font-weight: bold;
  font-size: 1.08rem;
  border-radius: 12px;
  width: 20px;
  height: 35px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(37, 99, 235, 0.13);
  letter-spacing: 1px;
  transition: transform 0.18s;
  flex-direction: column;
  z-index: 2;
}

.most-viewed-card:hover .rank-badge-capsule-vertical {
  transform: scaleY(1.12) scaleX(1.08);
}

.most-viewed-name {
  color: #c7cfe6;
  font-size: 1.08rem;
  font-weight: 500;
  margin-top: 28px;
  margin-bottom: 8px;
  text-align: center;
  padding: 0 8px;
  word-break: break-all;
}
</style>
