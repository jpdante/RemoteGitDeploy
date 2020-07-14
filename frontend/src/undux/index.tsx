import {
  EffectsAs,
  Store,
  createConnectedStoreAs,
  withReduxDevtools,
} from "undux";
import { hasToken } from "../services/auth";
import { withEffects } from "./effects";

type AuthState = {
  isLogged: boolean;
  token: string | null;
};

type ProfileState = {
  username: string | null;
  email: string | null;
};

let initialAuthState: AuthState = {
  isLogged: hasToken(),
  token: null
};

let initialProfileState: ProfileState = {
  username: localStorage.getItem("profile.username") || "guest",
  email: localStorage.getItem("profile.email") || "guest@email.com"
};

export default createConnectedStoreAs(
  {
    auth: initialAuthState,
    profile: initialProfileState
  },
  (stores) => {
    return withEffects({
      auth: withReduxDevtools(stores.auth),
      profile: withReduxDevtools(stores.profile)
    });
  }
);

export type StoreProps = {
  auth: Store<AuthState>;
  profile: Store<ProfileState>;
};

export type StoreEffects = EffectsAs<{
  auth: AuthState;
  profile: ProfileState;
}>;
