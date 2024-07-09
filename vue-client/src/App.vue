<script setup lang="ts">
import { ref } from 'vue';

interface weather {
  date: Date;
  temperatureC: Number;
  temperatureF: Number;
  summary: String;
}

const data = ref<weather[]>([]);
const getData = () => fetch("http://localhost:5182/WeatherForecast").then(async response => {
  const reader = response.body?.getReader();
  if (!reader) {
    return;
  }
  const decoder = new TextDecoder();
  while (true) {
    const { done, value } = await reader.read();
    if (done) break;
    var item = decoder.decode(value).replace(/\[|]/g, '').replace(/^,/, '');
    var parsedItem: weather = JSON.parse(item) as weather;
    parsedItem.date = new Date(parsedItem.date)

    data.value.push(parsedItem)
  }
  reader.releaseLock();
});

getData();

function formatDate(date: Date){
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-based
  const year = date.getFullYear();

  return `${day}/${month}/${year}`;
}

</script>

<template>
  <div class="container">
    <table class="table table-striped table-bordered">
      <thead>
        <tr>
          <th>Date</th>
          <th>TemperatureC</th>
          <th>TemperatureF</th>
          <th>Summary</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in data">
          <td>{{ formatDate(item.date) }}</td>
          <td>{{ item.temperatureC }}</td>
          <td>{{ item.temperatureF }}</td>
          <td>{{ item.summary }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.container {
  width: 100%;
}

th {
  width: 200px;
  text-align: left;
}
</style>
