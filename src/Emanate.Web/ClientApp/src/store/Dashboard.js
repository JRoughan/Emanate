const requestDisplayConfigurationsType = 'REQUEST_DISPLAY_CONFIGURATIONS';
const receiveDisplayConfigurationsType = 'RECEIVE_DISPLAY_CONFIGURATIONS';
const displayConfigurationAddedType = 'DISPLAY_CONFIGURATION_ADDED';
const displayConfigurationRemovedType = 'DISPLAY_CONFIGURATION_REMOVED';
const displayConfigurationUpdatedType = 'DISPLAY_CONFIGURATION_UPDATED';

const initialState = {
    displayConfigurations: [],
    isLoadingDisplayConfigurations: false,
};

export const actionCreators = {

    requestDisplayConfigurations: () => async (dispatch) => {
        dispatch({ type: requestDisplayConfigurationsType });

        const url = 'api/DisplayConfigurations';
        const response = await fetch(url);
        const configurations = await response.json();

        dispatch({ type: receiveDisplayConfigurationsType, configurations });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === requestDisplayConfigurationsType) {
        return {
            ...state,
            isLoadingDisplayConfigurations: true
        };
    }

    if (action.type === receiveDisplayConfigurationsType) {
        return {
            ...state,
            displayConfigurations: action.configurations,
            isLoadingDisplayConfigurations: false
        };
    }

    if (action.type === displayConfigurationAddedType) {
        return {
            ...state,
            displayConfigurations: [...state.displayConfigurations, action.newDisplayConfiguration]
        };
    }

    if (action.type === displayConfigurationRemovedType) {
        return {
            ...state,
            displayConfigurations: [...state.displayConfigurations.filter(c => c.id !== action.oldDisplayConfigurationId)]
        };
    }

    if (action.type === displayConfigurationUpdatedType) {
        return {
            ...state,
            displayConfigurations: state.displayConfigurations.map((config) => {
                if (config.id === action.updatedConfiguration.id) {
                    return {
                        ...config,
                        ...action.updatedConfiguration
                    }
                }

                return config;
            })
        };
    }

    return state;
};
