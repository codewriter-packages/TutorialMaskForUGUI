# TutorialMaskForUGUI [![Github license](https://img.shields.io/github/license/codewriter-packages/TutorialMaskForUGUI.svg?style=flat-square)](#) [![Unity 2021.3](https://img.shields.io/badge/Unity-2021.3+-2296F3.svg?style=flat-square)](#) ![GitHub package.json version](https://img.shields.io/github/package-json/v/codewriter-packages/TutorialMaskForUGUI?style=flat-square)

UI Tutorial Mask is a component for highlighting specific objects over darkened background for Unity UI (uGUI)

![Demo](https://github.com/codewriter-packages/TutorialMaskForUGUI/assets/26966368/4b31beed-3324-42d8-ad7c-62edcafb1bdc)

## :rocket: How to use?

- Create an `Image` that will cover the entire visible area, add a `TutorialMask` script to it
- Add a `TutorialObject` script to each object that should be visible on top of the image
- If you want to use `TutorialObject` with `Mask`, you need to add `MaskFixForTutorial` script on each mask object. It's necessary due to a bug in the mask component that overwrites all bit (even which it does not use) in stencil buffer

## :open_book: How to Install
Minimal Unity Version is 2021.3.

Library distributed as git package ([How to install package from git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html))
<br>Git URL: `https://github.com/codewriter-packages/TutorialMaskForUGUI.git`

## :green_book: License

TutorialMaskForUGUI is [MIT licensed](./LICENSE.md).
