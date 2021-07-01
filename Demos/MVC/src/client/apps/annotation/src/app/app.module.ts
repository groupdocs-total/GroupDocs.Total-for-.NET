import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { AnnotationModule } from "@groupdocs.examples.angular/annotation";

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule,
    AnnotationModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
