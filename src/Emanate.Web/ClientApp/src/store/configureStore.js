import { applyMiddleware, combineReducers, compose, createStore } from 'redux';
import thunk from 'redux-thunk';
import { routerReducer, routerMiddleware } from 'react-router-redux';
import * as Counter from './Counter';
import * as WeatherForecasts from './WeatherForecasts';
import * as Admin from './Admin';
import * as SignalR from '@aspnet/signalr';

export default function configureStore(history, initialState) {
    const reducers = {
        counter: Counter.reducer,
        weatherForecasts: WeatherForecasts.reducer,
        admin: Admin.reducer
    };

    const middleware = [
        thunk,
        routerMiddleware(history),
        signalRInvokeMiddleware
    ];

    // In development, use the browser's Redux dev tools extension if installed
    const enhancers = [];
    const isDevelopment = process.env.NODE_ENV === 'development';
    if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
        enhancers.push(window.devToolsExtension());
    }

    const rootReducer = combineReducers({
        ...reducers,
        routing: routerReducer
    });

    const store = createStore(
        rootReducer,
        initialState,
        compose(applyMiddleware(...middleware), ...enhancers)
    );

    signalRRegisterCommands(store);

    return store;
}

const connection = new SignalR.HubConnectionBuilder()
    .withUrl("/counterx")
    .configureLogging(SignalR.LogLevel.Information)
    .build();

export function signalRInvokeMiddleware(store: any) {
    return (next: any) => async (action: any) => {
        switch (action.type) {
            case "SIGNALR_INCREMENT_COUNT":
                connection.invoke('IncrementCounter');
                break;
            case "SIGNALR_DECREMENT_COUNT":
                connection.invoke('DecrementCounter');
                break;
        }

        return next(action);
    }
}

export function signalRRegisterCommands(store: any, callback: Function) {

    connection.on('IncrementCounter', data => {
        store.dispatch({ type: 'INCREMENT_COUNT' })
        console.log("Count has been incremented");
    })

    connection.on('DecrementCounter', data => {
        store.dispatch({ type: 'DECREMENT_COUNT' })
        console.log("Count has been decremented");
    })

    connection.start().then(function () {
        console.log("connected");
    });
}





