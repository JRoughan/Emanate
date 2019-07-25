import { confirmAlert } from 'react-confirm-alert';
import 'react-confirm-alert/src/react-confirm-alert.css';

const initialState = { displayDevices: [], isLoadingDisplayDevices: false };

export const actionCreators = {

    removeDisplayDevice: (id) => async () => {
        confirmAlert({
            title: 'Delete Display Device?',
            message: 'Are you sure you want to delete this display device? This cannot be undone.',
            buttons: [
                {
                    label: 'Cancel'
                },
                {
                    label: 'Delete',
                    onClick: async () => {
                        const url = 'api/DisplayDevices/' + id;
                        await fetch(url, {
                            method: 'delete'
                        });
                    }
                }
            ]
        });
    }
};

export const reducer = (state) => {
    state = state || initialState;

    return state;
};
