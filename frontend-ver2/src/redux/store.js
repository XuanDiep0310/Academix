import { combineReducers, configureStore } from "@reduxjs/toolkit";
import accountReducer from "../redux/account/accountSlice";

// Gộp các reducer lại
const rootReducer = combineReducers({
  account: accountReducer,
});

// Tạo store cơ bản
const store = configureStore({
  reducer: rootReducer,
});

export { store };
