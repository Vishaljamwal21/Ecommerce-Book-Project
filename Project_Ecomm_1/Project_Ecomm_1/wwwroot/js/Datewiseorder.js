//let minDate = new DateTime('#min', {
//    format: 'MMMM Do YYYY'
//});
//let maxDate = new DateTime('#max', {
//    format: 'MMMM Do YYYY'
//});

// DataTables initialisation
let table = new DataTable('#example');

// Custom filtering function which will search data in column four between two values
DataTable.ext.search.push(function (settings, data, dataIndex) {
    let min = minDate.val();
    let max = maxDate.val();
    let date = new Date(data[4]);

    if (
        (min === null && max === null) ||
        (min === null && date <= max) ||
        (min <= date && max === null) ||
        (min <= date && date <= max)
    ) {
        return true;
    }
    return false;
});

// Refilter the table
document.querySelectorAll('#min, #max').forEach((el) => {
    el.addEventListener('change', () => table.draw());
});