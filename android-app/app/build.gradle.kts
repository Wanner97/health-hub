import groovy.json.JsonSlurper
import org.gradle.api.GradleException

plugins {
    alias(libs.plugins.android.application)
    alias(libs.plugins.kotlin.compose)
}

fun readVersions(): Triple<String, String, Int> {
    val versionsFile = rootProject.projectDir.parentFile.resolve("versions.json")

    if (!versionsFile.exists()) {
        throw GradleException("versions.json nicht gefunden: ${versionsFile.absolutePath}")
    }

    val json = JsonSlurper().parseText(versionsFile.readText()) as Map<*, *>

    val suite = json["suite"] as? Map<*, *>
        ?: throw GradleException("Block 'suite' fehlt in versions.json")

    val services = json["services"] as? Map<*, *>
        ?: throw GradleException("Block 'services' fehlt in versions.json")

    val android = services["android"] as? Map<*, *>
        ?: throw GradleException("Block 'services.android' fehlt in versions.json")

    val suiteVersion = suite["version"] as? String
        ?: throw GradleException("suite.version fehlt in versions.json")

    val androidVersion = android["version"] as? String
        ?: throw GradleException("services.android.version fehlt in versions.json")

    val androidVersionCode = (android["versionCode"] as? Number)?.toInt()
        ?: throw GradleException("services.android.versionCode fehlt oder ist ungültig")

    return Triple(suiteVersion, androidVersion, androidVersionCode)
}

val (suiteVersion, androidServiceVersion, androidServiceVersionCode) = readVersions()

android {
    namespace = "ch.claudiowanner.healthdataexporter"
    compileSdk {
        version = release(36) {
            minorApiLevel = 1
        }
    }

    defaultConfig {
        applicationId = "ch.claudiowanner.healthdataexporter"
        minSdk = 26
        targetSdk = 36
        versionCode = androidServiceVersionCode
        versionName = androidServiceVersion

        buildConfigField("String", "SUITE_VERSION", "\"$suiteVersion\"")
        buildConfigField("String", "SERVICE_VERSION", "\"$androidServiceVersion\"")

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"
    }

    buildTypes {
        release {
            isMinifyEnabled = false
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),
                "proguard-rules.pro"
            )
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11
        targetCompatibility = JavaVersion.VERSION_11
    }

    buildFeatures {
        compose = true
        buildConfig = true
    }
}

dependencies {
    implementation(libs.androidx.core.ktx)
    implementation(libs.androidx.lifecycle.runtime.ktx)
    implementation(libs.androidx.activity.compose)
    implementation(platform(libs.androidx.compose.bom))
    implementation(libs.androidx.compose.ui)
    implementation(libs.androidx.compose.ui.graphics)
    implementation(libs.androidx.compose.ui.tooling.preview)
    implementation(libs.androidx.compose.material3)
    implementation("com.google.android.material:material:1.12.0")

    implementation("com.google.code.gson:gson:2.11.0")

    implementation("androidx.health.connect:connect-client:1.1.0")

    testImplementation(libs.junit)
    androidTestImplementation(libs.androidx.junit)
    androidTestImplementation(libs.androidx.espresso.core)
    androidTestImplementation(platform(libs.androidx.compose.bom))
    androidTestImplementation(libs.androidx.compose.ui.test.junit4)
    debugImplementation(libs.androidx.compose.ui.tooling)
    debugImplementation(libs.androidx.compose.ui.test.manifest)
}