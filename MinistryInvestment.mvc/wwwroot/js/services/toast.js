export default function showToast(message, type = 'success') {
    const bgColor = type === 'success' ? 'bg-success-subtle' : 'bg-danger-subtle';
    const toastHtml = `
        <div id="liveToast" class="toast ${bgColor}" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header ${bgColor}">
                <strong class="me-auto">${type === 'success' ? 'Success' : 'Error'}</strong>
                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>`;
    const container = document.querySelector('.toast-container');
    container.insertAdjacentHTML('beforeend', toastHtml);

    const toastElement = container.lastElementChild;
    const toast = new bootstrap.Toast(toastElement);
    toast.show();

    toastElement.addEventListener('hidden.bs.toast', () => {
        toastElement.remove();
    })
}
