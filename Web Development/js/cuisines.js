
import { PopulatePage } from './jsonHandler.js';

const articlesContainer = document.getElementById('articles');

const PopulateCuisines = (data) => {
    console.log(data);

    data.forEach(cuisine => {
        const article = document.createElement('article');
        article.className = 'article';
    
        article.innerHTML = `
            <div class="article-wrapper">
              <figure>
                <img src="${cuisine.image}" alt="${cuisine.name}" />
              </figure>
              <div class="article-body">
                <h2>${cuisine.name}</h2>
                <p>${cuisine.description}</p>
                <a href="./ViewReceipies.html" class="read-more">
            
                </a>
              </div>
            </div>
          `;
    
        articlesContainer.appendChild(article);
    });
}

PopulatePage('cuisines', PopulateCuisines);

