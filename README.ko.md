# eXtraTMP (XTMP)

eXtraTMP는 TextMeshPro(TMP)에 커스텀 대체 폰트(Fallback Fonts)를 손쉽게 추가할 수 있도록 설계된 가벼운 BepInEx 플러그인입니다.

## 사용 가이드

### 1. 폰트 에셋 준비
1. Unity의 [Font Asset Creator](https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/FontAssetsCreator.html)를 사용하여 폰트 아틀라스를 생성합니다.
2. Unity [AssetBundle Browser](https://github.com/Unity-Technologies/AssetBundles-Browser)를 사용하여 하나 이상의 폰트 에셋을 단일 에셋 번들 파일로 패키징합니다.

### 2. 설치 및 설정
1. 생성된 에셋 번들 파일을 `BepInEx/plugins/XTMP/bundles` 폴더에 위치시킵니다.
2. 동일한 폴더에 번들 파일과 이름이 같고 확장자가 `.ini`인 설정 파일을 생성합니다. (예: 번들 이름이 `myfonts.bundle`인 경우 `myfonts.ini`로 생성)
3. 아래의 문법을 참고하여 `.ini` 파일에 대체 폰트 규칙을 작성합니다.

> **참고:** 이 플러그인은 간소화된 파서를 사용합니다. 형태는 INI과 유사하지만, 완전한 문법을 지원하지는 않습니다.

### 설정 파일 문법

```ini
# 전역 대체 폰트 (GLOBAL FALLBACK)
# 섹션 헤더 없이 나열된 폰트들은 TMP_Settings의 전역 대체 폰트 목록에 자동으로 추가됩니다.
Galmuri7

# 폰트 직접 교체 (DIRECT REPLACEMENT)
# '->' 기호를 사용하여 지정한 원본 폰트를 새로운 폰트로 완전히 교체합니다.
# 문법: [원본_폰트명 -> 교체할_폰트명]
[ExperimentIEatCrayons -> KCCKimHoon SDF]

# 특정 폰트 지정 대체 (TARGETED FALLBACKS)
# 대괄호([]) 안에 원본 폰트명을 섹션 헤더로 지정하여, 해당 폰트에만 적용될 대체 폰트를 설정합니다.
[Retro Gaming]
NeoDunggeunmoPro

# 한 섹션 아래에 여러 개의 폰트를 나열하면, 작성된 순서에 따라 순차적으로 대체 폰트가 적용됩니다.
[LiberationSans SDF]
NotoSansKR SDF
NotoSansJP SDF
