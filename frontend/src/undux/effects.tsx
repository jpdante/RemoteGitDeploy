import { StoreEffects } from "./index";
import { deleteToken, hasToken, setToken } from './../services/auth';

export const withEffects: StoreEffects = ({ auth, profile }) => {
  // AUTH

  auth.on("token").subscribe((next) => {
    if (next === undefined || next === null) {
      deleteToken();
      auth.set("isLogged")(false);
    } else {
      setToken(next);
      auth.set("isLogged")(hasToken());
    }
  });

  // PROFILE

  profile.on("username").subscribe((next) => {
    if(next !== null) localStorage.setItem("profile.username", next);
  });

  return { auth, profile };
};