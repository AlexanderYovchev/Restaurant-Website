const tableStates = {};

function toggleTableState(tableId) {
    const button = document.getElementById('table-' + tableId);
    let currentState = button.classList.contains('empty') ? 'empty' :
        button.classList.contains('reserved') ? 'reserved' : 'taken';

    // Cycle through the states: empty -> reserved -> taken -> empty
    let newState;
    if (currentState === 'empty') {
        newState = 'reserved';
        button.classList.remove('empty');
        button.classList.add('reserved');
    } else if (currentState === 'reserved') {
        newState = 'taken';
        button.classList.remove('reserved');
        button.classList.add('taken');
    } else if (currentState === 'taken') {
        newState = 'empty';
        button.classList.remove('taken');
        button.classList.add('empty');
    }

    // Store the new state in the tableStates object
    tableStates[tableId] = newState;
}

function confirmTableStates() {
    // Make an AJAX call to the server to save the table states
    fetch('/Table/UpdateTableStates', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': '@Antiforgery.GetTokens(Context).RequestToken'
        },
        body: JSON.stringify(tableStates)
    })
        .then(response => {
            if (response.ok) {
                alert("Table states updated successfully!");
            } else {
                alert("Error updating table states.");
            }
        })
        .catch(error => console.error('Error:', error));
}