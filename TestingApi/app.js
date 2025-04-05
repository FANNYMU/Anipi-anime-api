// AnimeHub Application Logic

// API Configuration
const API_BASE_URL = 'http://localhost:5235/api/v1';
const ENDPOINTS = {
  anime: `${API_BASE_URL}/anime`,
  types: `${API_BASE_URL}/types`,
  statuses: `${API_BASE_URL}/statuses`,
  seasons: `${API_BASE_URL}/seasons`,
  years: `${API_BASE_URL}/years`,
  tags: `${API_BASE_URL}/tags`,
};

// State Management
const state = {
  animeList: [],
  filters: {
    title: '',
    type: '',
    status: '',
    year: '',
    season: '',
    tag: '',
    sortBy: 'title',
    sortDescending: false,
  },
  pagination: {
    page: 1,
    pageSize: 20,
    totalPages: 0,
    totalCount: 0,
  },
  metadata: {
    types: [],
    statuses: [],
    seasons: [],
    years: [],
    tags: [],
  },
  loading: false,
  error: null,
};

// DOM Elements
const elements = {
  animeGrid: document.getElementById('anime-grid'),
  searchInput: document.getElementById('search-input'),
  searchButton: document.getElementById('search-button'),
  typeSelect: document.getElementById('type-select'),
  statusSelect: document.getElementById('status-select'),
  yearSelect: document.getElementById('year-select'),
  seasonSelect: document.getElementById('season-select'),
  tagSelect: document.getElementById('tag-select'),
  sortSelect: document.getElementById('sort-select'),
  sortOrderToggle: document.getElementById('sort-order'),
  pagination: document.getElementById('pagination'),
  loadingIndicator: document.getElementById('loading'),
  errorContainer: document.getElementById('error'),
  modalBackdrop: document.getElementById('modal-backdrop'),
  modalContent: document.getElementById('modal-content'),
  modalClose: document.getElementById('modal-close'),
};

// Initialize the application
async function initApp() {
  try {
    // Show loading indicator
    showLoading(true);
    
    // Load metadata for filters
    await Promise.all([
      loadMetadata('types'),
      loadMetadata('statuses'),
      loadMetadata('seasons'),
      loadMetadata('years'),
      loadMetadata('tags'),
    ]);
    
    // Populate filter dropdowns
    populateFilters();
    
    // Load initial anime data
    await fetchAnime();
    
    // Set up event listeners
    setupEventListeners();
    
  } catch (error) {
    showError('Failed to initialize the application. Please refresh the page.');
    console.error('Initialization error:', error);
  } finally {
    showLoading(false);
  }
}

// Fetch anime data based on current filters and pagination
async function fetchAnime() {
  try {
    showLoading(true);
    state.error = null;
    
    // Build query parameters
    const queryParams = new URLSearchParams();
    queryParams.append('page', state.pagination.page);
    queryParams.append('pageSize', state.pagination.pageSize);
    
    // Add filters if they have values
    Object.entries(state.filters).forEach(([key, value]) => {
      if (value) queryParams.append(key, value);
    });
    
    // Fetch data from API
    const response = await fetch(`${ENDPOINTS.anime}?${queryParams.toString()}`);
    
    if (!response.ok) {
      throw new Error(`API error: ${response.status}`);
    }
    
    const result = await response.json();
    
    if (!result.success) {
      throw new Error(result.message || 'Unknown API error');
    }
    
    // Update state with new data
    state.animeList = result.data;
    state.pagination = {
      ...state.pagination,
      ...result.pagination,
    };
    
    // Render the anime list and pagination
    renderAnimeList();
    renderPagination();
    
  } catch (error) {
    showError('Failed to fetch anime data. Please try again later.');
    console.error('Fetch error:', error);
  } finally {
    showLoading(false);
  }
}

// Load metadata for filters (types, statuses, etc.)
async function loadMetadata(type) {
  try {
    const response = await fetch(ENDPOINTS[type]);
    
    if (!response.ok) {
      throw new Error(`API error: ${response.status}`);
    }
    
    const result = await response.json();
    
    if (!result.success) {
      throw new Error(result.message || 'Unknown API error');
    }
    
    state.metadata[type] = result.data;
    
  } catch (error) {
    console.error(`Error loading ${type} metadata:`, error);
  }
}

// Populate filter dropdowns with metadata
function populateFilters() {
  // Populate type filter
  populateSelect(elements.typeSelect, state.metadata.types, 'All Types');
  
  // Populate status filter
  populateSelect(elements.statusSelect, state.metadata.statuses, 'All Statuses');
  
  // Populate year filter
  populateSelect(elements.yearSelect, state.metadata.years, 'All Years');
  
  // Populate season filter
  populateSelect(elements.seasonSelect, state.metadata.seasons, 'All Seasons');
  
  // Populate tag filter
  populateSelect(elements.tagSelect, state.metadata.tags, 'All Tags');
}

// Helper function to populate select elements
function populateSelect(selectElement, options, defaultLabel) {
  if (!selectElement) return;
  
  // Clear existing options
  selectElement.innerHTML = '';
  
  // Add default option
  const defaultOption = document.createElement('option');
  defaultOption.value = '';
  defaultOption.textContent = defaultLabel;
  selectElement.appendChild(defaultOption);
  
  // Add options from metadata
  options.forEach(option => {
    const optionElement = document.createElement('option');
    optionElement.value = option;
    optionElement.textContent = option;
    selectElement.appendChild(optionElement);
  });
}

// Render anime list to the DOM
function renderAnimeList() {
  if (!elements.animeGrid) return;
  
  // Clear existing content
  elements.animeGrid.innerHTML = '';
  
  if (state.animeList.length === 0) {
    elements.animeGrid.innerHTML = '<div class="no-results">No anime found matching your criteria.</div>';
    return;
  }
  
  // Create and append anime cards
  state.animeList.forEach(anime => {
    const animeCard = createAnimeCard(anime);
    elements.animeGrid.appendChild(animeCard);
  });
}

// Create an anime card element
function createAnimeCard(anime) {
  const card = document.createElement('div');
  card.className = 'anime-card';
  card.dataset.id = anime.id;
  
  // Get image with fallback
  const imageUrl = anime.picture || 'placeholder.jpg';
  
  // Format tags (limit to 3 for display)
  const tags = anime.tags && anime.tags.length > 0 
    ? anime.tags.slice(0, 3).map(tag => `<span class="tag">${tag}</span>`).join('') 
    : '';
  
  // Format score
  const score = anime.score && anime.score.arithmeticMean 
    ? `<span class="score">${anime.score.arithmeticMean.toFixed(2)}</span>` 
    : '';
  
  // Create card HTML
  card.innerHTML = `
    <img src="${imageUrl}" alt="${anime.title}" class="anime-image" onerror="this.src='placeholder.jpg'">
    <div class="anime-info">
      <h3 class="anime-title">${anime.title}</h3>
      <div class="anime-meta">
        <span>${anime.type || 'Unknown'}</span>
        ${score}
      </div>
      <div class="anime-meta">
        <span>${anime.animeSeason ? `${anime.animeSeason.season} ${anime.animeSeason.year}` : 'Unknown'}</span>
        <span>${anime.episodes ? `${anime.episodes} eps` : 'Unknown'}</span>
      </div>
      <div class="anime-tags">
        ${tags}
      </div>
    </div>
  `;
  
  // Add click event to open modal with details
  card.addEventListener('click', () => openAnimeModal(anime));
  
  return card;
}

// Render pagination controls
function renderPagination() {
  if (!elements.pagination) return;
  
  elements.pagination.innerHTML = '';
  
  const { page, totalPages } = state.pagination;
  
  // Previous button
  const prevButton = document.createElement('button');
  prevButton.textContent = '← Previous';
  prevButton.disabled = page <= 1;
  prevButton.addEventListener('click', () => {
    if (page > 1) {
      state.pagination.page--;
      fetchAnime();
    }
  });
  
  // Next button
  const nextButton = document.createElement('button');
  nextButton.textContent = 'Next →';
  nextButton.disabled = page >= totalPages;
  nextButton.addEventListener('click', () => {
    if (page < totalPages) {
      state.pagination.page++;
      fetchAnime();
    }
  });
  
  // Page indicator
  const pageIndicator = document.createElement('span');
  pageIndicator.className = 'page-indicator';
  pageIndicator.textContent = `Page ${page} of ${totalPages}`;
  
  // Append pagination elements
  elements.pagination.appendChild(prevButton);
  elements.pagination.appendChild(pageIndicator);
  elements.pagination.appendChild(nextButton);
}

// Open modal with anime details
function openAnimeModal(anime) {
  if (!elements.modalBackdrop || !elements.modalContent) return;
  
  // Format details
  const synonyms = anime.synonyms && anime.synonyms.length > 0 
    ? anime.synonyms.join(', ') 
    : 'None';
  
  const tags = anime.tags && anime.tags.length > 0 
    ? anime.tags.map(tag => `<span class="tag">${tag}</span>`).join('') 
    : 'None';
  
  const score = anime.score && anime.score.arithmeticMean 
    ? anime.score.arithmeticMean.toFixed(2) 
    : 'N/A';
  
  const duration = anime.duration 
    ? `${anime.duration.value} ${anime.duration.unit}` 
    : 'Unknown';
  
  // Set modal content
  elements.modalContent.innerHTML = `
    <button id="modal-close" class="modal-close">×</button>
    <div class="modal-header">
      <h2 class="modal-title">${anime.title}</h2>
    </div>
    <div class="modal-body">
      <div class="modal-image">
        <img src="${anime.picture || 'placeholder.jpg'}" alt="${anime.title}" onerror="this.src='placeholder.jpg'">
      </div>
      <div class="modal-details">
        <div class="detail-group">
          <div class="detail-label">Type:</div>
          <div>${anime.type || 'Unknown'}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Episodes:</div>
          <div>${anime.episodes || 'Unknown'}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Status:</div>
          <div>${anime.status || 'Unknown'}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Season:</div>
          <div>${anime.animeSeason ? `${anime.animeSeason.season} ${anime.animeSeason.year}` : 'Unknown'}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Duration:</div>
          <div>${duration}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Score:</div>
          <div>${score}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Synonyms:</div>
          <div>${synonyms}</div>
        </div>
        <div class="detail-group">
          <div class="detail-label">Tags:</div>
          <div class="modal-tags">${tags}</div>
        </div>
      </div>
    </div>
  `;
  
  // Show modal
  elements.modalBackdrop.classList.add('active');
  
  // Set up close button
  const closeButton = document.getElementById('modal-close');
  if (closeButton) {
    closeButton.addEventListener('click', closeAnimeModal);
  }
  
  // Close on backdrop click
  elements.modalBackdrop.addEventListener('click', (e) => {
    if (e.target === elements.modalBackdrop) {
      closeAnimeModal();
    }
  });
  
  // Prevent scrolling on body
  document.body.style.overflow = 'hidden';
}

// Close anime details modal
function closeAnimeModal() {
  if (!elements.modalBackdrop) return;
  
  elements.modalBackdrop.classList.remove('active');
  document.body.style.overflow = '';
}

// Set up event listeners for filters and search
function setupEventListeners() {
  // Search button click
  if (elements.searchButton) {
    elements.searchButton.addEventListener('click', () => {
      state.filters.title = elements.searchInput.value.trim();
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Search input enter key
  if (elements.searchInput) {
    elements.searchInput.addEventListener('keypress', (e) => {
      if (e.key === 'Enter') {
        state.filters.title = elements.searchInput.value.trim();
        state.pagination.page = 1;
        fetchAnime();
      }
    });
  }
  
  // Type filter change
  if (elements.typeSelect) {
    elements.typeSelect.addEventListener('change', () => {
      state.filters.type = elements.typeSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Status filter change
  if (elements.statusSelect) {
    elements.statusSelect.addEventListener('change', () => {
      state.filters.status = elements.statusSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Year filter change
  if (elements.yearSelect) {
    elements.yearSelect.addEventListener('change', () => {
      state.filters.year = elements.yearSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Season filter change
  if (elements.seasonSelect) {
    elements.seasonSelect.addEventListener('change', () => {
      state.filters.season = elements.seasonSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Tag filter change
  if (elements.tagSelect) {
    elements.tagSelect.addEventListener('change', () => {
      state.filters.tag = elements.tagSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Sort select change
  if (elements.sortSelect) {
    elements.sortSelect.addEventListener('change', () => {
      state.filters.sortBy = elements.sortSelect.value;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
  
  // Sort order toggle
  if (elements.sortOrderToggle) {
    elements.sortOrderToggle.addEventListener('change', () => {
      state.filters.sortDescending = elements.sortOrderToggle.checked;
      state.pagination.page = 1;
      fetchAnime();
    });
  }
}

// Show/hide loading indicator
function showLoading(isLoading) {
  state.loading = isLoading;
  
  if (elements.loadingIndicator) {
    elements.loadingIndicator.style.display = isLoading ? 'block' : 'none';
  }
  
  if (elements.animeGrid) {
    elements.animeGrid.style.opacity = isLoading ? '0.5' : '1';
  }
}

// Show error message
function showError(message) {
  state.error = message;
  
  if (elements.errorContainer) {
    elements.errorContainer.textContent = message;
    elements.errorContainer.style.display = message ? 'block' : 'none';
  }
}

// Initialize the application when DOM is loaded
document.addEventListener('DOMContentLoaded', initApp);
