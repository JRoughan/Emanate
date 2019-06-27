const signalRincrementCountType = 'SIGNALR_INCREMENT_COUNT';
const signalRdecrementCountType = 'SIGNALR_DECREMENT_COUNT';
const incrementCountType = 'INCREMENT_COUNT';
const decrementCountType = 'DECREMENT_COUNT';
const initialState = { count: 0 };

export const actionCreators = {
  increment: () => ({ type: signalRincrementCountType }),
  decrement: () => ({ type: signalRdecrementCountType })
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === incrementCountType) {
    return { ...state, count: state.count + 1 };
  }

  if (action.type === decrementCountType) {
    return { ...state, count: state.count - 1 };
  }

  return state;
};
