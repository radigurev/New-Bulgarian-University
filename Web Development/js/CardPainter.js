export const CreateCard = (data, articlesContainer, page, filter) => {
  const article = document.createElement('article');
  article.className = 'article';

  article.innerHTML = `
        <div class="article-wrapper">
          <figure>
            <img src="${data.image}" alt="${data.name}" />
          </figure>
          <div class="article-body">
            <h2>${data.name}</h2>
            <p>${data.description}</p>
            <a href="./${page}.html?${filter}" class="read-more">
        
            </a>
          </div>
        </div>
      `;

  articlesContainer.appendChild(article);
} 